using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Manager Settings")]
    public bool handelEscapeInput = true;     // ESC키 입력 처리 여부를 설정하는 변수

    [SerializeField] GameObject optionUI;


    private Dictionary<UIType, BaseUI> registeredUI = new Dictionary<UIType, BaseUI>();     // 등록된 UI들을 타입별로 저장하는 딕셔너리
    private Stack<BaseUI> uiStack = new Stack<BaseUI>();           // 열린 UI들의 스택 (최상위 UI 추적용)
    // private List<BaseUI> openedUI = new List<BaseUI>(); // uiStack으로 충분하므로 List는 제거 가능성이 높음


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        RegisterAllUI(); // 씬 로드 시 모든 BaseUI 컴포넌트들을 찾아서 딕셔너리에 등록
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
        // 이는 씬 전환 시 기존 UI들이 파괴되므로 필요합니다.
        // DontDestroyOnLoad 된 UIManager 자체는 파괴되지 않습니다.
        registeredUI.Clear();
        // uiStack은 씬 전환 시 유지되어야 하는 UI가 있을 수도 있으니 주의하여 관리합니다.
        // 여기서는 모든 UI가 씬에 따라 파괴되고 다시 등록된다고 가정하겠습니다.
        // 만약 DontDestroyOnLoad 되는 UI가 있다면 해당 UI는 스택에서 제외해야 합니다.
        uiStack.Clear(); // 씬이 바뀌면 열려있던 UI들이 대부분 파괴되므로 스택도 비웁니다.
        // openedUI.Clear(); // List가 사용되지 않으면 이 줄도 필요 없음

        RegisterAllUI();
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
        // UIManager가 DontDestroyOnLoad되면, 씬이 로드될 때마다 해당 씬의 UI들을 찾아서 등록합니다.
        BaseUI[] uis = FindObjectsByType<BaseUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (BaseUI ui in uis)
        {
            // MainMenuUI는 BaseUI를 상속받지 않으므로 여기에 등록되지 않습니다.
            // OptionUI, InventoryUI 등만 등록됩니다.
            RegisterUI(ui);
        }
    }

    // UIManager 내부에서 UI를 딕셔너리에 등록하는 용도
    public void RegisterUI(BaseUI ui)
    {
        if (ui.UIType != UIType.None && !registeredUI.ContainsKey(ui.UIType))     // UI타입이 None이 아니고, 미등록 상태일 때
        {
            registeredUI[ui.UIType] = ui;                                         // 딕셔너리에 등록
            // Debug.Log($"[UIManager] UI 등록됨: {ui.UIType}");
        }
        else if (ui.UIType != UIType.None && registeredUI.ContainsKey(ui.UIType))
        {
            // Debug.LogWarning($"[UIManager] 이미 등록된 UI: {ui.UIType}. 중복 등록 시도 방지.");
        }
    }

    // BaseUI.OpenUI()에서 호출되어 UI를 스택에 추가
    public void RegisterOpenedUI(BaseUI ui)
    {
        if (!uiStack.Contains(ui)) // uiStack.Contains()는 O(N) 이지만, 스택 사이즈가 크지 않다면 괜찮습니다.
        {
            uiStack.Push(ui);           // UI 스택에 추가 (최상위로)
            // Debug.Log($"[UIManager] UI 스택에 추가됨: {ui.UIType}. 현재 스택 크기: {uiStack.Count}");
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
        else // 스택의 최상위가 아닌 UI가 닫힐 경우 (예: 특정 상황에서 강제로 닫는 경우)
        {
            // 이 로직은 스택의 LIFO 원칙을 깨뜨릴 수 있으므로 신중하게 사용해야 합니다.
            // 보통 UI 스택은 최상위 UI만 닫을 수 있도록 구현하는 것이 일반적입니다.
            // 그러나 스택 중간의 UI를 닫아야 한다면 다음과 같이 처리할 수 있습니다.
            // 단, 이 경우 스택 재구성 비용이 발생합니다.
            var tempList = uiStack.ToList(); // 스택을 리스트로 변환
            if (tempList.Remove(ui)) // 리스트에서 해당 UI 제거
            {
                uiStack.Clear(); // 스택 비우기
                // Z-Order (겹쳐지는 순서)를 유지하려면, 리스트를 다시 스택에 넣을 때 역순으로 넣어야 합니다.
                // 또는 UILayer를 사용하여 정렬된 리스트를 유지하는 것이 더 좋습니다.
                // 여기서는 간단히 다시 스택에 넣습니다. (UI Layer를 고려하지 않음)
                foreach (var item in tempList.Reverse<BaseUI>()) // 원래 스택의 역순으로 다시 Push (LIFO 유지)
                {
                    uiStack.Push(item);
                }
                // Debug.Log($"[UIManager] UI 스택 중간에서 제거됨: {ui.UIType}. 현재 스택 크기: {uiStack.Count}");
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
            // MainMenuUI가 BaseUI를 상속받지 않으므로 topUI가 null일 때입니다.
            // 이 경우 옵션 UI를 엽니다.
            OpenUI(UIType.Option); // UIManager의 OpenUI 메서드 사용
        }
    }
}