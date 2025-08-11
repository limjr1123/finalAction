using System.Collections.Generic;  // Dictionary, Stack 등 컬렉션 사용을 위한 네임스페이스
using UnityEngine;               // Unity 엔진의 기본 클래스들 사용을 위한 네임스페이스
using UnityEngine.SceneManagement; // 씬 관리 기능 사용을 위한 네임스페이스
using System.Linq;               // LINQ 확장 메서드 사용을 위한 네임스페이스 (ToList, Reverse 등)

// UIManager 클래스: 게임 내 모든 UI를 중앙에서 관리하는 싱글톤 매니저
public class UIManager : Singleton<UIManager>
{
    // === 설정 변수들 ===
    [Header("UI Manager Settings")]      // Inspector에서 "UI Manager Settings" 헤더로 그룹화
    public bool handelEscapeInput = true; // ESC키 입력 처리 여부를 설정하는 변수 (기본값: true)

    [SerializeField] GameObject optionUI; // 옵션 UI 게임오브젝트 참조 (직접 할당용)

    // === 게임 UI 참조들 ===
    [Header("Game UI References")]                    // Inspector에서 "Game UI References" 헤더로 그룹화
    [SerializeField] HUD hudController;               // HUD 스크립트 직접 참조 (Inspector에서 할당)
    [SerializeField] MainMenuUI mainMenuController;   // MainMenuUI 스크립트 직접 참조 (Inspector에서 할당)

    // === 내부 관리 변수들 ===
    private Dictionary<UIType, BaseUI> registeredUI = new Dictionary<UIType, BaseUI>(); // 등록된 모든 UI들을 UIType별로 저장하는 딕셔너리
    private Stack<BaseUI> uiStack = new Stack<BaseUI>();                                // 현재 열린 UI들의 스택 (LIFO 구조로 최상위 UI 추적)

    // Unity의 Awake 메서드 (싱글톤 초기화보다 늦게 실행되도록 override)
    protected override void Awake()
    {
        base.Awake();  // 부모 클래스(Singleton)의 Awake 먼저 호출하여 싱글톤 초기화

        // 씬이 로드될 때마다 호출될 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 현재 씬의 모든 BaseUI 컴포넌트들을 찾아서 딕셔너리에 등록
        RegisterAllUI();
    }

    // Unity의 Start 메서드 (게임 시작 시 한 번 호출)
    void Start()
    {
        InitializeHUD();  // 게임 시작 시 즉시 HUD 초기화 시도
    }

    // Unity의 Update 메서드 (매 프레임마다 호출)
    void Update()
    {
        // 조건부 컴파일: 에디터 또는 PC 플랫폼에서만 실행
#if UNITY_EDITOR || UNITY_STANDALONE
        // ESC키 입력 처리가 활성화되어 있고 ESC키가 눌렸다면
        if (handelEscapeInput && Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeInput(); // ESC키 입력 처리 함수 호출
        }
#endif
    }

    // Unity의 OnDestroy 메서드 (오브젝트가 파괴될 때 호출)
    void OnDestroy()
    {
        // 씬 로드 이벤트 구독 해제 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때마다 호출되는 이벤트 핸들러
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 기존 UI 등록 정보를 모두 초기화 (새로운 씬의 UI들로 갱신하기 위해)
        registeredUI.Clear(); // 등록된 UI 딕셔너리 비우기
        uiStack.Clear();      // UI 스택 비우기

        RegisterAllUI(); // 새로운 씬의 모든 BaseUI 컴포넌트들을 찾아서 다시 등록

        // 씬 로드 후 HUD 초기화 (약간의 지연을 두고 실행)
        StartCoroutine(DelayedHUDInitialization());
    }

    // 씬 로드 후 약간의 지연을 두고 HUD를 초기화하는 코루틴
    private System.Collections.IEnumerator DelayedHUDInitialization()
    {
        yield return new WaitForEndOfFrame(); // 현재 프레임이 끝날 때까지 대기 (모든 오브젝트가 완전히 로드되도록)
        InitializeHUD(); // HUD 초기화 실행
    }

    // HUD를 초기화하는 메서드 (PlayerStats와 연결)
    private void InitializeHUD()
    {
        Debug.Log("UIManager: HUD 초기화 시작"); // HUD 초기화 시작 로그

        // HUD 컨트롤러가 Inspector에서 할당되지 않았다면 자동으로 찾기
        if (hudController == null)
        {
            hudController = FindAnyObjectByType<HUD>(); // 씬에서 HUD 컴포넌트 자동 검색
            Debug.Log($"HUD 자동 검색 결과: {hudController != null}"); // 검색 결과 로그
        }

        // PlayerStats 컴포넌트를 씬에서 찾기
        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();
        Debug.Log($"PlayerStats 검색 결과: {playerStats != null}"); // 검색 결과 로그

        // HUD와 PlayerStats가 모두 존재한다면
        if (hudController != null && playerStats != null)
        {
            // HUD에 PlayerStats 컴포넌트를 직접 전달하여 초기화
            hudController.InitializeWithPlayer(playerStats);
            Debug.Log("UIManager: HUD와 PlayerStats 연결 완료!"); // 연결 성공 로그
        }
        else
        {
            // 초기화 실패 시 경고 로그 (어떤 컴포넌트가 없는지 표시)
            Debug.LogWarning($"HUD 초기화 실패 - HUD: {hudController != null}, PlayerStats: {playerStats != null}");
        }
    }

    // GameManager에서 캐릭터 로드가 완료되었을 때 호출될 공개 메서드
    public void OnCharacterLoaded()
    {
        Debug.Log("UIManager: 캐릭터 로드 완료, 게임 UI 활성화 시작"); // 캐릭터 로드 완료 로그
        InitializeHUD(); // HUD 다시 초기화 (새로 로드된 캐릭터 정보로)
    }

    // 특정 타입의 UI를 열고 해당 UI 컴포넌트를 반환하는 공개 메서드
    public BaseUI OpenUI(UIType uiType)
    {
        // 딕셔너리에서 해당 타입의 UI가 등록되어 있는지 확인
        if (registeredUI.TryGetValue(uiType, out BaseUI ui))
        {
            ui.OpenUI(); // BaseUI의 OpenUI() 메서드 호출 (이 메서드가 자동으로 스택에 추가함)
            return ui;   // 열린 UI 컴포넌트 반환
        }
        else
        {
            // 등록되지 않은 UI 타입인 경우 경고 로그 출력
            Debug.LogWarning($"[UIManager] 등록된 UI가 없습니다: {uiType}");
            return null; // null 반환
        }
    }

    // 특정 타입의 UI를 닫는 공개 메서드
    public void CloseUI(UIType uiType)
    {
        // 딕셔너리에서 해당 타입의 UI가 등록되어 있는지 확인
        if (registeredUI.TryGetValue(uiType, out BaseUI ui))
        {
            ui.CloseUI(); // BaseUI의 CloseUI() 메서드 호출 (이 메서드가 자동으로 스택에서 제거함)
        }
        else
        {
            // 등록되지 않은 UI를 닫으려고 시도한 경우 경고 로그 출력
            Debug.LogWarning($"[UIManager] 등록되지 않은 UI를 닫으려 시도: {uiType}");
        }
    }

    // 현재 씬의 모든 BaseUI 컴포넌트들을 찾아서 딕셔너리에 등록하는 메서드
    private void RegisterAllUI()
    {
        // 현재 활성화/비활성화 상태에 관계없이 모든 BaseUI 컴포넌트를 찾기
        // FindObjectsInactive.Include: 비활성화된 오브젝트도 포함해서 검색
        BaseUI[] uis = FindObjectsByType<BaseUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // 찾은 모든 BaseUI 컴포넌트들을 하나씩 등록
        foreach (BaseUI ui in uis)
        {
            RegisterUI(ui); // 개별 UI 등록 메서드 호출
        }
    }

    // 개별 UI를 딕셔너리에 등록하는 공개 메서드 (외부에서도 호출 가능)
    public void RegisterUI(BaseUI ui)
    {
        // UI 타입이 None이 아니고, 아직 딕셔너리에 등록되지 않은 경우에만 등록
        if (ui.UIType != UIType.None && !registeredUI.ContainsKey(ui.UIType))
        {
            registeredUI[ui.UIType] = ui; // 딕셔너리에 UIType을 키로 하여 UI 등록
        }
    }

    // BaseUI.OpenUI()에서 호출되어 열린 UI를 스택에 추가하는 메서드
    public void RegisterOpenedUI(BaseUI ui)
    {
        // 스택에 이미 해당 UI가 있는지 확인 (중복 방지)
        if (!uiStack.Contains(ui))
        {
            uiStack.Push(ui); // UI를 스택의 최상위에 추가 (LIFO 구조)
        }
    }

    // BaseUI.CloseUI()에서 호출되어 닫힌 UI를 스택에서 제거하는 메서드
    public void UnRegisterCloseUI(BaseUI ui)
    {
        // 스택의 최상위 UI를 닫는 일반적인 경우
        if (uiStack.Count > 0 && uiStack.Peek() == ui)
        {
            uiStack.Pop(); // 최상위 UI를 스택에서 제거 (Pop)
        }
        else // 스택의 최상위가 아닌 중간의 UI가 닫힐 경우 (특수한 경우)
        {
            var tempList = uiStack.ToList(); // 스택을 임시 리스트로 변환
            if (tempList.Remove(ui))         // 리스트에서 해당 UI 제거 (성공하면 true 반환)
            {
                uiStack.Clear(); // 기존 스택 완전히 비우기
                // 원래 스택의 순서를 유지하면서 다시 Push (LIFO 구조 유지)
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
        if (uiStack.Count > 0)    // 스택에 UI가 하나 이상 있다면
            return uiStack.Peek(); // 최상단 UI 반환 (제거하지 않고 조회만)

        return null; // 열린 UI가 없으면 null 반환
    }

    // ESC키 입력을 처리하는 메서드
    private void HandleEscapeInput()
    {
        BaseUI topUI = GetTopUI(); // 현재 최상위 UI 가져오기

        // 최상위 UI가 있고 해당 UI가 ESC키 처리를 허용한다면
        if (topUI != null && topUI.CanHandleEscape())
        {
            topUI.CloseUI(); // 해당 UI 닫기
        }
        else if (topUI == null) // 열린 UI가 전혀 없다면
        {
            // 옵션 UI 열기 (UIType.Option은 미리 정의된 UI 타입)
            OpenUI(UIType.Option);
        }
        // 최상위 UI가 있지만 ESC키 처리를 허용하지 않는 경우는 아무것도 하지 않음
    }
}