using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;

// UIManager 클래스: 게임 내 모든 UI를 중앙에서 관리하는 싱글톤 매니저
public class UIManager : Singleton<UIManager>
{
    // === 설정 변수들 ===
    [Header("UI Manager Settings")]
    public bool handelEscapeInput = true;

    [SerializeField] GameObject optionUI;

    // ⭐ 씬별 UI 제어 설정 추가
    [Header("Scene-specific UI Control")]
    [SerializeField] private List<string> uiDisabledScenes = new List<string>();
    [SerializeField] private CanvasGroup mainCanvasGroup;
    [Tooltip("이 씬들에서는 UIManager만 활성화되고 자식 UI들은 모두 비활성화됩니다")]

    // === 게임 UI 참조들 ===
    [Header("Game UI References")]
    [SerializeField] HUD hudController;
    [SerializeField] MainMenuUI mainMenuController;

    // === 내부 관리 변수들 ===
    private Dictionary<UIType, BaseUI> registeredUI = new Dictionary<UIType, BaseUI>();
    private Stack<BaseUI> uiStack = new Stack<BaseUI>();
    private bool isUIDisabledScene = false;
    private Dictionary<GameObject, bool> originalChildStates = new Dictionary<GameObject, bool>();

    // Unity의 Awake 메서드 (싱글톤 초기화보다 늦게 실행되도록 override)
    protected override void Awake()
    {
        base.Awake();

        // 씬이 로드될 때마다 호출될 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ⭐ 순서 변경: 먼저 씬별 UI 제어 확인
        CheckCurrentSceneUIControl();

        // ⭐ 그 다음에 UI 등록
        RegisterAllUI();
    }

    // Unity의 Start 메서드 (게임 시작 시 한 번 호출)
    void Start()
    {
        if (!isUIDisabledScene)
        {
            InitializeHUD();
        }
    }

    // Unity의 Update 메서드 (매 프레임마다 호출)
    void Update()
    {
        // ⭐ ESC 입력은 항상 처리하되, 디버깅 로그 추가
        if (handelEscapeInput && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log($"[UIManager] ESC 키 입력 감지! 현재 씬: {SceneManager.GetActiveScene().name}");
            Debug.Log($"[UIManager] isUIDisabledScene: {isUIDisabledScene}");
            Debug.Log($"[UIManager] handelEscapeInput: {handelEscapeInput}");

            HandleEscapeInput();
        }
    }

    // Unity의 OnDestroy 메서드 (오브젝트가 파괴될 때 호출)
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ⭐ 현재 씬이 UI 비활성화 씬인지 확인하는 메서드
    private void CheckCurrentSceneUIControl()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        bool shouldDisableUI = uiDisabledScenes.Contains(currentSceneName);

        Debug.Log($"[UIManager] 씬: {currentSceneName}, 이전 isUIDisabledScene: {isUIDisabledScene}, 새로운 값: {shouldDisableUI}");
        Debug.Log($"[UIManager] uiDisabledScenes 목록: {string.Join(", ", uiDisabledScenes)}");

        if (shouldDisableUI != isUIDisabledScene)
        {
            isUIDisabledScene = shouldDisableUI;

            if (mainCanvasGroup != null)
            {
                if (isUIDisabledScene)
                {
                    // ⭐ UI 비활성화 씬에서는 시각적으로만 숨기기 (상호작용은 유지)
                    mainCanvasGroup.alpha = 0f;
                    mainCanvasGroup.interactable = false;
                    // ⭐ blocksRaycasts는 false로 두지 않음 (ESC 입력을 위해)
                    // mainCanvasGroup.blocksRaycasts = false;
                    Debug.Log($"[UIManager] '{currentSceneName}' 씬에서 UI를 숨겼습니다.");
                }
                else
                {
                    mainCanvasGroup.alpha = 1f;
                    mainCanvasGroup.interactable = true;
                    mainCanvasGroup.blocksRaycasts = true;
                    Debug.Log($"[UIManager] '{currentSceneName}' 씬에서 UI를 표시했습니다.");
                }
            }
        }
        else
        {
            Debug.Log($"[UIManager] '{currentSceneName}' 씬에서 UI 상태 변경 없음 (isUIDisabledScene: {isUIDisabledScene})");
        }
    }

    // ⭐ 외부에서 특정 씬을 UI 비활성화 씬으로 추가하는 공개 메서드
    public void AddUIDisabledScene(string sceneName)
    {
        if (!uiDisabledScenes.Contains(sceneName))
        {
            uiDisabledScenes.Add(sceneName);
            Debug.Log($"[UIManager] '{sceneName}' 씬이 UI 비활성화 씬 목록에 추가되었습니다.");
        }
    }

    // ⭐ 외부에서 특정 씬을 UI 비활성화 씬에서 제거하는 공개 메서드
    public void RemoveUIDisabledScene(string sceneName)
    {
        if (uiDisabledScenes.Remove(sceneName))
        {
            Debug.Log($"[UIManager] '{sceneName}' 씬이 UI 비활성화 씬 목록에서 제거되었습니다.");
        }
    }

    // ⭐ 현재 씬이 UI 비활성화 씬인지 확인하는 공개 메서드
    public bool IsUIDisabledScene()
    {
        return isUIDisabledScene;
    }

    // ⭐ 씬이 로드될 때마다 호출되는 이벤트 핸들러 - 실행 순서 수정
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[UIManager] OnSceneLoaded 시작 - 씬: {scene.name}");

        // ⭐ 먼저 모든 열린 UI를 닫기 (씬 전환 시 UI 상태 초기화)
        CloseAllOpenUI();

        // 기존 UI 등록 정보를 모두 초기화
        registeredUI.Clear();
        uiStack.Clear();

        // ⭐ 순서 변경: 먼저 씬별 UI 제어 확인
        CheckCurrentSceneUIControl();

        // ⭐ 그 다음에 UI 등록 (이제 isUIDisabledScene이 올바르게 설정됨)
        RegisterAllUI();

        // UI가 비활성화된 씬이 아닐 때만 HUD 초기화
        if (!isUIDisabledScene)
        {
            StartCoroutine(DelayedHUDInitialization());
        }

        Debug.Log($"[UIManager] OnSceneLoaded 완료 - 등록된 UI 개수: {registeredUI.Count}");
    }

    // ⭐ 모든 열린 UI를 닫는 메서드 추가
    private void CloseAllOpenUI()
    {
        Debug.Log($"[UIManager] 모든 열린 UI 닫기 - 현재 스택 개수: {uiStack.Count}");

        // 스택에 있는 모든 UI를 닫기
        while (uiStack.Count > 0)
        {
            BaseUI topUI = uiStack.Pop();
            if (topUI != null && topUI.gameObject.activeInHierarchy)
            {
                topUI.gameObject.SetActive(false); // 강제로 비활성화
                Debug.Log($"[UIManager] {topUI.UIType} UI 강제 닫기");
            }
        }

        // 혹시 놓친 UI들을 위해 모든 BaseUI 컴포넌트 확인
        BaseUI[] allUIs = FindObjectsByType<BaseUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (BaseUI ui in allUIs)
        {
            if (ui != null && ui.gameObject.activeInHierarchy && ui.UIType != UIType.None)
            {
                ui.gameObject.SetActive(false);
                Debug.Log($"[UIManager] {ui.UIType} UI 추가 강제 닫기");
            }
        }

        uiStack.Clear(); // 스택 완전히 비우기
    }

    // 씬 로드 후 약간의 지연을 두고 HUD를 초기화하는 코루틴
    private IEnumerator DelayedHUDInitialization()
    {
        yield return new WaitForEndOfFrame();
        InitializeHUD();
    }

    // HUD를 초기화하는 메서드 (PlayerStats와 연결)
    private void InitializeHUD()
    {
        if (isUIDisabledScene) return;

        Debug.Log("UIManager: HUD 초기화 시작");

        if (hudController == null)
        {
            hudController = FindAnyObjectByType<HUD>();
            Debug.Log($"HUD 자동 검색 결과: {hudController != null}");
        }

        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();
        Debug.Log($"PlayerStats 검색 결과: {playerStats != null}");

        if (hudController != null && playerStats != null)
        {
            hudController.InitializeWithPlayer(playerStats);
            Debug.Log("UIManager: HUD와 PlayerStats 연결 완료!");
        }
        else
        {
            Debug.LogWarning($"HUD 초기화 실패 - HUD: {hudController != null}, PlayerStats: {playerStats != null}");
        }
    }

    // GameManager에서 캐릭터 로드가 완료되었을 때 호출될 공개 메서드
    public void OnCharacterLoaded()
    {
        if (isUIDisabledScene) return;

        Debug.Log("UIManager: 캐릭터 로드 완료, 게임 UI 활성화 시작");
        InitializeHUD();
    }

    // 특정 타입의 UI를 열고 해당 UI 컴포넌트를 반환하는 공개 메서드
    public BaseUI OpenUI(UIType uiType)
    {
        // ⭐ 옵션 UI는 예외적으로 항상 열 수 있음
        if (uiType == UIType.Option)
        {
            // ⭐ 등록된 옵션 UI가 파괴되었는지 확인
            if (registeredUI.TryGetValue(uiType, out BaseUI optionUI))
            {
                if (optionUI == null) // 파괴된 경우
                {
                    Debug.LogWarning("[UIManager] 등록된 옵션 UI가 파괴됨, 재등록 시도");
                    registeredUI.Remove(uiType); // 딕셔너리에서 제거
                    ForceRegisterOptionUI(); // 다시 찾아서 등록

                    // 재등록 후 다시 시도
                    if (registeredUI.TryGetValue(uiType, out optionUI) && optionUI != null)
                    {
                        optionUI.OpenUI();
                        return optionUI;
                    }
                }
                else // 유효한 경우
                {
                    optionUI.OpenUI();
                    return optionUI;
                }
            }

            // 등록되지 않았거나 재등록 실패한 경우
            Debug.LogWarning($"[UIManager] 옵션 UI를 찾을 수 없습니다!");
            ForceRegisterOptionUI(); // 한 번 더 시도
            if (registeredUI.TryGetValue(uiType, out optionUI) && optionUI != null)
            {
                optionUI.OpenUI();
                return optionUI;
            }
            return null;
        }

        // UI가 비활성화된 씬에서는 다른 UI를 열지 않음
        if (isUIDisabledScene)
        {
            Debug.LogWarning($"[UIManager] UI가 비활성화된 씬에서는 UI를 열 수 없습니다: {uiType}");
            return null;
        }

        // ⭐ 다른 UI들도 파괴 체크 추가
        if (registeredUI.TryGetValue(uiType, out BaseUI ui))
        {
            if (ui == null) // 파괴된 경우
            {
                Debug.LogWarning($"[UIManager] 등록된 {uiType} UI가 파괴됨, 딕셔너리에서 제거");
                registeredUI.Remove(uiType);
                return null;
            }

            ui.OpenUI();
            return ui;
        }
        else
        {
            Debug.LogWarning($"[UIManager] 등록된 UI가 없습니다: {uiType}");
            return null;
        }
    }

    // 특정 타입의 UI를 닫는 공개 메서드
    public void CloseUI(UIType uiType)
    {
        if (registeredUI.TryGetValue(uiType, out BaseUI ui))
        {
            ui.CloseUI();
        }
        else
        {
            Debug.LogWarning($"[UIManager] 등록되지 않은 UI를 닫으려 시도: {uiType}");
        }
    }

    // ⭐ 현재 씬의 모든 BaseUI 컴포넌트들을 찾아서 딕셔너리에 등록하는 메서드 - 디버깅 로그 추가
    private void RegisterAllUI()
    {
        Debug.Log($"[UIManager] RegisterAllUI 호출 - isUIDisabledScene: {isUIDisabledScene}");

        // 현재 활성화/비활성화 상태에 관계없이 모든 BaseUI 컴포넌트를 찾기
        BaseUI[] uis = FindObjectsByType<BaseUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"[UIManager] 찾은 BaseUI 개수: {uis.Length}");

        // 찾은 모든 BaseUI 컴포넌트들을 하나씩 등록
        foreach (BaseUI ui in uis)
        {
            Debug.Log($"[UIManager] UI 등록 시도: {ui.UIType}");
            RegisterUI(ui);
        }

        Debug.Log($"[UIManager] 최종 등록된 UI 개수: {registeredUI.Count}");
        foreach (var kvp in registeredUI)
        {
            Debug.Log($"[UIManager] 등록된 UI: {kvp.Key}");
        }
    }

    // 개별 UI를 딕셔너리에 등록하는 공개 메서드 (외부에서도 호출 가능)
    public void RegisterUI(BaseUI ui)
    {
        // ⭐ null 체크 추가
        if (ui == null)
        {
            Debug.LogWarning("[UIManager] null UI를 등록하려 시도함");
            return;
        }

        // 옵션 UI는 예외적으로 항상 등록
        if (ui.UIType == UIType.Option)
        {
            if (!registeredUI.ContainsKey(ui.UIType))
            {
                registeredUI[ui.UIType] = ui;
                Debug.Log($"[UIManager] 옵션 UI 강제 등록 완료");
            }
            return;
        }

        // UI가 비활성화된 씬에서는 다른 UI 등록하지 않음
        if (isUIDisabledScene)
        {
            Debug.Log($"[UIManager] UI 비활성화 씬이므로 {ui.UIType} UI 등록을 건너뜁니다.");
            return;
        }

        if (ui.UIType != UIType.None && !registeredUI.ContainsKey(ui.UIType))
        {
            registeredUI[ui.UIType] = ui;
            Debug.Log($"[UIManager] {ui.UIType} UI 등록 완료");
        }
    }

    // BaseUI.OpenUI()에서 호출되어 열린 UI를 스택에 추가하는 메서드
    public void RegisterOpenedUI(BaseUI ui)
    {
        // ⭐ 옵션 UI는 예외적으로 항상 스택에 추가 가능
        if (ui.UIType == UIType.Option)
        {
            if (!uiStack.Contains(ui))
            {
                uiStack.Push(ui);
            }
            return;
        }

        // UI가 비활성화된 씬에서는 다른 UI를 스택에 추가하지 않음
        if (isUIDisabledScene) return;

        if (!uiStack.Contains(ui))
        {
            uiStack.Push(ui);
        }
    }

    // BaseUI.CloseUI()에서 호출되어 닫힌 UI를 스택에서 제거하는 메서드
    public void UnRegisterCloseUI(BaseUI ui)
    {
        if (uiStack.Count > 0 && uiStack.Peek() == ui)
        {
            uiStack.Pop();
        }
        else
        {
            var tempList = uiStack.ToList();
            if (tempList.Remove(ui))
            {
                uiStack.Clear();
                foreach (var item in tempList.Reverse<BaseUI>())
                {
                    uiStack.Push(item);
                }
            }
        }
    }

    // 현재 최상위에 있는 UI를 반환하는 공개 메서드
    public BaseUI GetTopUI()
    {
        if (uiStack.Count > 0)
            return uiStack.Peek();

        return null;
    }

    // ESC키 입력을 처리하는 메서드
    private void HandleEscapeInput()
    {
        Debug.Log($"[UIManager] ESC 입력 감지 - 현재 씬: {SceneManager.GetActiveScene().name}, UI비활성화씬: {isUIDisabledScene}");

        BaseUI topUI = GetTopUI();

        if (topUI != null && topUI.CanHandleEscape())
        {
            Debug.Log($"[UIManager] 최상위 UI 닫기: {topUI.UIType}");
            topUI.CloseUI();
        }
        else if (topUI == null)
        {
            Debug.Log("[UIManager] ESC키로 옵션 UI 열기 시도");
            // ⭐ 옵션 UI가 등록되지 않았다면 강제로 등록 시도
            if (!registeredUI.ContainsKey(UIType.Option))
            {
                Debug.Log("[UIManager] 옵션 UI가 등록되지 않음, 강제 등록 시도");
                ForceRegisterOptionUI();
            }
            OpenUI(UIType.Option);
        }
    }

    // ⭐ 옵션 UI를 강제로 등록하는 메서드
    private void ForceRegisterOptionUI()
    {
        Debug.Log("[UIManager] 옵션 UI 강제 등록 시도");
        BaseUI[] allUIs = FindObjectsByType<BaseUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (BaseUI ui in allUIs)
        {
            if (ui != null && ui.UIType == UIType.Option)
            {
                registeredUI[UIType.Option] = ui;
                Debug.Log("[UIManager] 옵션 UI 강제 등록 완료");
                return;
            }
        }

        Debug.LogError("[UIManager] 옵션 UI를 찾을 수 없습니다! 씬에 OptionUI가 존재하는지 확인하세요.");
    }
}