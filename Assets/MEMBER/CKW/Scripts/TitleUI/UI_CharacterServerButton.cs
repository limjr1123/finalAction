using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameSave;

public class UI_CharacterServerButton : MonoBehaviour
{

    [Header("Button Settings")]
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button serverChangeButton;
    [SerializeField] Button characterCreationButton;
    [SerializeField] Button characterDeleteButton;


    [SerializeField] GameObject SelectCharacterWindow;
    [SerializeField] GameObject characterSelectionWindow;



    private CharacterData selectedCharacterData;
    private int selectedCharacterIndex;


    void Start()
    {

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);

        if (serverChangeButton != null)
            serverChangeButton.onClick.AddListener(ServerChange);

        if (characterCreationButton != null)
            characterCreationButton.onClick.AddListener(CharacterCreate);

        if (characterDeleteButton != null)
            characterDeleteButton.onClick.AddListener(CharacterDelete);
    }


    void OnEnable()
    {
        // 캐릭터 선택 이벤트 구독
        CharacterInfoToggles.OnCharacterSelected += OnCharacterSelectedHandler;
    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        CharacterInfoToggles.OnCharacterSelected -= OnCharacterSelectedHandler;
    }

    private void OnCharacterSelectedHandler(CharacterData character, int index)
    {
        // 'OnCharacterSelectedHandler'는 캐릭터가 선택될 때 호출되는 이벤트 핸들러 메서드입니다.
        selectedCharacterData = character;
        // 선택된 캐릭터의 데이터를 'selectedCharacterData' 변수에 할당합니다.
        selectedCharacterIndex = index;
        // 선택된 캐릭터의 인덱스를 'selectedCharacterIndex' 변수에 할당합니다.
        Debug.Log($"선택된 캐릭터: {character.characterName}, 인덱스: {index}");
    }

    private void StartGame()
    {
        // 선택된 캐릭터가 있는지 확인
        if (selectedCharacterData == null)
        {
            selectedCharacterData = CharacterInfoToggles.GetCurrentSelectedCharacter();
            // 'CharacterInfoToggles' 클래스에서 현재 선택된 캐릭터 정보를 가져와 'selectedCharacterData'에 할당합니다.
        }

        if (selectedCharacterData != null)
        {
            Debug.Log($"게임 시작: {selectedCharacterData.characterName}");

            // 게임 데이터에 선택된 캐릭터 설정 (이미 CharacterInfoToggles에서 설정됨)
            // GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(selectedCharacterIndex);

            // 게임 씬 로드
            SceneManager.LoadScene("Field"); // 실제 게임 씬 이름으로 변경
        }
        else
        {
            // 'selectedCharacterData'가 여전히 null인 경우 (즉, 캐릭터가 선택되지 않은 경우)
            Debug.LogWarning("선택된 캐릭터가 없습니다!");
            // 디버그 콘솔에 경고 메시지를 출력합니다.
        }
    }


    private void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    private void ServerChange()
    {
        SceneManager.LoadScene("CKW_TitleScene");
    }


    private void CharacterCreate()
    {
        SelectCharacterWindow.SetActive(false);
        characterSelectionWindow.SetActive(true);
    }


    private void CharacterDelete()
    {

    }

}
