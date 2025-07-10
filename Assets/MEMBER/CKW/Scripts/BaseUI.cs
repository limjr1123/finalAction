using System;
using UnityEngine;

public enum UIType    // UI타입
{
    None,
    MainMenu, // MainMenu는 BaseUI를 상속받지 않기로 했으므로 여기서는 사용되지 않을 가능성이 높습니다.
    Option,
    Inventory,
    Shop,
    Settings,
    CharacterInfo, // 추가: CharacterInfo UI 타입
    SkillWindow    // 추가: SkillWindow UI 타입

}

public enum UILayer     // UI레이어
{
    Background = 0,
    Normal = 100,
    Popup = 200,
    System = 300,
    Alert = 400
}

public class BaseUI : MonoBehaviour
{
    [Header("Base UI Settings")]
    public UIType UIType = UIType.None;
    public UILayer uILayer = UILayer.Background;
    public bool closeOnEscape = true;


    public Action<BaseUI> UIOpened;
    public Action<BaseUI> UIClosed;


    // Start는 필요한 경우 자식 클래스에서 오버라이드.
    void Start()
    {
        // UIManager의 Awake에서 FindObjectsByType으로 모든 BaseUI를 찾아서 등록하기 때문에
        // Start에서는 별도의 등록 로직이 필요 없습니다.
    }


    public virtual void OpenUI()    // UI 열기
    {
        gameObject.SetActive(true);
        OnOpen();
        UIOpened?.Invoke(this);     // 이벤트 등록

        // UIManager의 RegisterAllUI에서 이미 registeredUI 딕셔너리에 등록될 것입니다.
        // 하지만 혹시 모를 경우를 위해 이 한줄을 다시 추가하는 것이 더 안전할 수 있습니다.
        UIManager.Instance?.RegisterUI(this); // Optional: UIManager.Awake의 RegisterAllUI가 확실히 작동한다면 이 줄은 생략 가능

        // 핵심: 스택에 추가하는 부분은 반드시 호출되어야 합니다.
        UIManager.Instance?.RegisterOpenedUI(this); // <--- 이 줄이 중요! (열린 UI 스택/리스트에 추가)
    }


    public virtual void CloseUI()  // UI 닫기
    {
        gameObject.SetActive(false);
        OnClose();
        UIClosed?.Invoke(this);     // 이벤트 등록
        UIManager.Instance?.UnRegisterCloseUI(this);
    }


    protected virtual void OnOpen() { }     // UI 열렸을 때 호출

    protected virtual void OnClose() { }    // UI 닫혔을 때 호출


    public bool IsActive() { return gameObject.activeInHierarchy; }     // 오브젝트가 계층구조에서 활성화 되어있는지 확인


    public virtual bool CanHandleEscape()           // ESC 처리 여부
    {
        return closeOnEscape && IsActive();     // ESC로 닫기 가능 + 현재 활성화 되어있는지 확인
    }
}