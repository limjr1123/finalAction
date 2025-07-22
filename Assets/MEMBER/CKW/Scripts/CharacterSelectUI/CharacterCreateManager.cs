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

    [Header("Character Jobs")]
    [SerializeField] JobData warriorJob;
    [SerializeField] JobData archerJob;
    [SerializeField] JobData mageJob;
    [SerializeField] JobData thiefJob;

    [Header("Button Settings")]
    [SerializeField] Button selectionJobButton;
    [SerializeField] Button backButton;

    [Header("Required Components")]
    [SerializeField] GameObject selectCharacterWindow;
    [SerializeField] GameObject characterCreationWindow;
    [SerializeField] GameObject nickNameCreationWindow;

    // 모든 패널을 관리하기 위한 배열
    public GameObject[] allCharacterPanels;

    // 현재 선택된 직업 데이터를 저장할 변수
    private JobData selectedJobData;

    // 싱글톤 인스턴스 (다른 스크립트에서 접근할 수 있도록)
    public static CharacterCreateManager Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 시작 시 모든 패널 숨기기
        HideAllPanels();

        // 각 토글의 OnValueChanged 이벤트에 함수 연결
        if (warriorToggle != null)
            warriorToggle.onValueChanged.AddListener((isOn) => OnCharacterToggleChanged(isOn, warriorPanel, warriorJob));
        if (archerToggle != null)
            archerToggle.onValueChanged.AddListener((isOn) => OnCharacterToggleChanged(isOn, archerPanel, archerJob));
        if (mageToggle != null)
            mageToggle.onValueChanged.AddListener((isOn) => OnCharacterToggleChanged(isOn, magePanel, mageJob));
        if (thiefToggle != null)
            thiefToggle.onValueChanged.AddListener((isOn) => OnCharacterToggleChanged(isOn, thiefPanel, thiefJob));

        if (backButton != null)
            backButton.onClick.AddListener(BackCharacterSelectionWindow);

        if (selectionJobButton != null)
            selectionJobButton.onClick.AddListener(NickNameCreate);
    }

    // 토글의 상태가 변경될 때 호출될 범용 함수 (JobData 추가)
    public void OnCharacterToggleChanged(bool isOn, GameObject panelToShow, JobData jobData)
    {
        if (isOn) // 토글이 켜졌을 때
        {
            HideAllPanels(); // 다른 모든 패널 숨기기
            if (panelToShow != null)
            {
                panelToShow.SetActive(true); // 해당 패널 활성화
            }

            // 선택된 직업 데이터 저장
            selectedJobData = jobData;
            Debug.Log($"선택된 직업: {jobData?.jobName}");
        }
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
        // 직업이 선택되었는지 확인
        if (selectedJobData == null)
        {
            Debug.LogWarning("직업을 먼저 선택해주세요!");
            return;
        }

        nickNameCreationWindow.SetActive(true);
    }

    // 선택된 직업 데이터를 반환하는 함수 (다른 스크립트에서 사용)
    public JobData GetSelectedJobData()
    {
        return selectedJobData;
    }
}