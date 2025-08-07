using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIManager : Singleton<UIManager>
{
    [Header("UI Manager Settings")]
    public bool handelEscapeInput = true;     // ESC키 입력 처리 여부를 설정하는 변수

    [SerializeField] GameObject optionUI;

    [Header("Game UI References")]
    [SerializeField] HUD hudController;           // HUD 스크립트 참조
    [SerializeField] MainMenuUI mainMenuController; // MainMenuUI 스크립트 참조

    private Dictionary<UIType, BaseUI> registeredUI = new Dictionary<UIType, BaseUI>();     // 등록된 UI들을 타입별로 저장하는 딕셔너리
    private Stack<BaseUI> uiStack = new Stack<BaseUI>();           // 열린 UI들의 스택 (최상위 UI 추적용)

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
        RegisterAllUI(); // 씬 로드 시 모든 BaseUI 컴포넌트들을 찾아서 딕셔너리에 등록
    }

    void Start()
    {
        // 시작 시 즉시 HUD 초기화 시도
        InitializeHUD();
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (handelEscapeInput && Input.GetKeyDown(KeyCode.Escape)) // ESC키 처리가 활성화되어 있고 ESC키가 눌렸다면
        {
            HandleEscapeInput();                                   // ESC키 입력 처리 함수 호출
        }
#endif
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때마다 기존 UI 등록을 초기화하고 다시 찾아서 등록합니다.
        registeredUI.Clear();
        uiStack.Clear();

        RegisterAllUI();

        // HUD 초기화 (씬 로드 후)
        StartCoroutine(DelayedHUDInitialization());
    }

    // 씬 로드 후 약간의 지연을 두고 HUD 초기화
    private System.Collections.IEnumerator DelayedHUDInitialization()
    {
        yield return new WaitForEndOfFrame();
        InitializeHUD();
    }

    // HUD 초기화 메서드
    private void InitializeHUD()
    {
        Debug.Log("UIManager: HUD 초기화 시작");

        // HUD 컨트롤러가 없으면 찾기
        if (hudController == null)
        {
            hudController = FindAnyObjectByType<HUD>();
            Debug.Log($"HUD 자동 검색 결과: {hudController != null}");
        }

        // PlayerStats 컴포넌트 찾기
        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();
        Debug.Log($"PlayerStats 검색 결과: {playerStats != null}");

        if (hudController != null && playerStats != null)
        {
            // HUD에 PlayerStats 컴포넌트를 직접 전달하여 초기화
            hudController.InitializeWithPlayer(playerStats);
            Debug.Log("UIManager: HUD와 PlayerStats 연결 완료!");
        }
        else
        {
            Debug.LogWarning($"HUD 초기화 실패 - HUD: {hudController != null}, PlayerStats: {playerStats != null}");
        }
    }

    // GameManager에서 캐릭터 로드 완료 시 호출될 메서드
    public void OnCharacterLoaded()
    {
        Debug.Log("UIManager: 캐릭터 로드 완료, 게임 UI 활성화 시작");
        InitializeHUD();
    }

    public BaseUI OpenUI(UIType uiType)
    {
        if (registeredUI.TryGetValue(uiType, out BaseUI ui))     // 해당 타입의 UI가 등록되어 있다면
        {
            ui.OpenUI(); // BaseUI의 OpenUI()를 호출하면 스택에 자동으로 추가됩니다.
            return ui;
        }
        else
        {
            Debug.LogWarning($"[UIManager] 등록된 UI가 없습니다: {uiType}"); // 경고 메시지
            return null;
        }
    }

    public void CloseUI(UIType uiType)
    {
        if (registeredUI.TryGetValue(uiType, out BaseUI ui))
        {
            ui.CloseUI(); // BaseUI의 CloseUI()를 호출하면 스택에서 자동으로 제거됩니다.
        }
        else
        {
            Debug.LogWarning($"[UIManager] 등록되지 않은 UI를 닫으려 시도: {uiType}");
        }
    }

    private void RegisterAllUI()      // 딕셔너리에 UI 등록하는 함수
    {
        // 현재 활성화/비활성화 상태에 관계없이 모든 BaseUI 컴포넌트를 찾습니다.
        BaseUI[] uis = FindObjectsByType<BaseUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (BaseUI ui in uis)
        {
            RegisterUI(ui);
        }
    }

    // UIManager 내부에서 UI를 딕셔너리에 등록하는 용도
    public void RegisterUI(BaseUI ui)
    {
        if (ui.UIType != UIType.None && !registeredUI.ContainsKey(ui.UIType))     // UI타입이 None이 아니고, 미등록 상태일 때
        {
            registeredUI[ui.UIType] = ui;                                         // 딕셔너리에 등록
        }
    }

    // BaseUI.OpenUI()에서 호출되어 UI를 스택에 추가
    public void RegisterOpenedUI(BaseUI ui)
    {
        if (!uiStack.Contains(ui))
        {
            uiStack.Push(ui);           // UI 스택에 추가 (최상위로)
        }
    }

    // BaseUI.CloseUI()에서 호출되어 UI를 스택에서 제거
    public void UnRegisterCloseUI(BaseUI ui)
    {
        // 스택의 최상위 UI를 닫는 일반적인 경우
        if (uiStack.Count > 0 && uiStack.Peek() == ui)
        {
            uiStack.Pop(); // 최상위 UI를 팝 (제거)
        }
        else // 스택의 최상위가 아닌 UI가 닫힐 경우
        {
            var tempList = uiStack.ToList(); // 스택을 리스트로 변환
            if (tempList.Remove(ui)) // 리스트에서 해당 UI 제거
            {
                uiStack.Clear(); // 스택 비우기
                foreach (var item in tempList.Reverse<BaseUI>()) // 원래 스택의 역순으로 다시 Push (LIFO 유지)
                {
                    uiStack.Push(item);
                }
            }
        }
    }

    public BaseUI GetTopUI()
    {
        if (uiStack.Count > 0)    // 스택에 UI가 등록되어 있으면
            return uiStack.Peek(); // 최상단 UI 반환

        return null;             // 열린 게 없으면 null 반환
    }

    private void HandleEscapeInput()
    {
        BaseUI topUI = GetTopUI(); // 현재 최상위 UI 가져오기

        if (topUI != null && topUI.CanHandleEscape()) // 최상위 UI가 있고 ESC키 처리가 가능하다면
        {
            topUI.CloseUI(); // 해당 UI 닫기
        }
        else if (topUI == null) // 열린 UI가 없다면
        {
            OpenUI(UIType.Option); // UIManager의 OpenUI 메서드 사용
        }
    }
}