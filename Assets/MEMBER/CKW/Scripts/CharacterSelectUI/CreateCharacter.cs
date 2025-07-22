using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateCharacter : MonoBehaviour
{
    [SerializeField] GameObject nicknameWindow;
    [SerializeField] Button nicknameButton;
    [SerializeField] Button closeButton;

    [Header("Nickname Input")]
    [SerializeField] TMP_InputField nicknameInputField; // 닉네임 입력 필드 추가

    void Start()
    {
        if (nicknameButton != null)
            nicknameButton.onClick.AddListener(CreateCharacters);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseNickname);

    }


    private void CreateCharacters()
    {
        // 닉네임 입력 확인
        if (nicknameInputField == null || string.IsNullOrEmpty(nicknameInputField.text.Trim()))
        {
            Debug.LogWarning("닉네임을 입력해주세요!");
            return;
        }

        // 선택된 직업 데이터 가져오기
        JobData selectedJob = CharacterCreateManager.Instance?.GetSelectedJobData();
        if (selectedJob == null)
        {
            Debug.LogWarning("직업이 선택되지 않았습니다!");
            return;
        }

        // 실제 캐릭터 생성
        string characterName = nicknameInputField.text.Trim();
        GameDataSaveLoadManager.Instance.CreateCharacter(characterName, selectedJob);

        Debug.Log($"캐릭터 생성 완료: {characterName}, 직업: {selectedJob.jobName}");

        RefreshCharacterUI();
        // 생성 완료 후 창 닫기 및 초기화
        CloseNickname();
        ResetCharacterCreation();
    }


    private void RefreshCharacterUI()
    {
        // CharacterInfoToggles 인스턴스 찾기
        CharacterInfoToggles characterInfoToggles = FindAnyObjectByType<CharacterInfoToggles>(FindObjectsInactive.Include);

        if (characterInfoToggles != null)
        {
            // 캐릭터 데이터 새로고침
            characterInfoToggles.RefreshCharacterData();

            // 새로 생성된 캐릭터를 자동 선택 (마지막 인덱스)
            int lastIndex = GameDataSaveLoadManager.Instance.GameData.characters.Count - 1;
            if (lastIndex >= 0)
            {
                characterInfoToggles.SelectCharacterByIndex(lastIndex);
            }

            Debug.Log("캐릭터 UI 업데이트 완료");
        }
        else
        {
            Debug.LogWarning("CharacterInfoToggles를 찾을 수 없습니다!");
        }
    }

    private void CloseNickname()
    {
        nicknameWindow.SetActive(false);
    }

    private void ResetCharacterCreation()
    {
        // 닉네임 입력 필드 초기화
        if (nicknameInputField != null)
            nicknameInputField.text = "";

        // 필요하다면 캐릭터 선택 화면으로 돌아가기
        // GameObject characterCreationWindow = CharacterCreateManager.Instance?.characterCreationWindow;
        // if (characterCreationWindow != null)
        //     characterCreationWindow.SetActive(false);
    }
}