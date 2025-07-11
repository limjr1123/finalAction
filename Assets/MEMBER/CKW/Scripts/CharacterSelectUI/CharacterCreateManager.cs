using UnityEngine;
using UnityEngine.UI;

public class CharacterCreateManager : MonoBehaviour
{
    [Header("Toggle Settings")]
    [SerializeField] Toggle warriorToggle;
    [SerializeField] Toggle archerToggle;
    [SerializeField] Toggle mageToggle;
    [SerializeField] Toggle thiefToggle;


    [Header("Panel Settings")]
    [SerializeField] GameObject warriorPanel;
    [SerializeField] GameObject archerPanel;
    [SerializeField] GameObject magePanel;
    [SerializeField] GameObject thiefPanel;


    [Header("Button Settings")]
    [SerializeField] Button selectionJobButton;
    [SerializeField] Button backButton;


    [Header("Required Components")]
    [SerializeField] GameObject selectCharacterWindow;
    [SerializeField] GameObject characterCreationWindow;
    [SerializeField] GameObject nickNameCreationWindow;



    // 모든 패널을 관리하기 위한 배열 (선택 사항이지만 편리)
    public GameObject[] allCharacterPanels;



    void Start()
    {
        // 시작 시 모든 패널 숨기기
        HideAllPanels();

        // 각 토글의 OnValueChanged 이벤트에 함수 연결
        if (warriorToggle != null)
            warriorToggle.onValueChanged.AddListener((isOn) => OnCharacterToggleChanged(isOn, warriorPanel));
        if (archerToggle != null)
            archerToggle.onValueChanged.AddListener((isOn) => OnCharacterToggleChanged(isOn, archerPanel));
        if (mageToggle != null)
            mageToggle.onValueChanged.AddListener((isOn) => OnCharacterToggleChanged(isOn, magePanel));
        if (thiefToggle != null)
            thiefToggle.onValueChanged.AddListener((isOn) => OnCharacterToggleChanged(isOn, thiefPanel));


        if (backButton != null)
            backButton.onClick.AddListener(BackCharacterSelectionWindow);


        if (selectionJobButton != null)     // 직업 생성 버튼을 눌렀을 때
            selectionJobButton.onClick.AddListener(NickNameCreate);


    }

    // 토글의 상태가 변경될 때 호출될 범용 함수
    public void OnCharacterToggleChanged(bool isOn, GameObject panelToShow)
    {
        if (isOn) // 토글이 켜졌을 때
        {
            HideAllPanels(); // 다른 모든 패널 숨기기
            if (panelToShow != null)
            {
                panelToShow.SetActive(true); // 해당 패널 활성화
            }
        }
        // 토글이 꺼질 때 (Toggle Group 사용 시 다른 토글이 켜지면서 자동으로 꺼지므로 별도 처리 불필요)
    }

    public void HideAllPanels()
    {
        foreach (GameObject panel in allCharacterPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }


    private void BackCharacterSelectionWindow()
    {
        selectCharacterWindow.SetActive(true);
        characterCreationWindow.SetActive(false);
    }


    private void NickNameCreate()
    {
        nickNameCreationWindow.SetActive(true);
    }
}
