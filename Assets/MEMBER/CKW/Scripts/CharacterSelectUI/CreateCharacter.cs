using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateCharacter : MonoBehaviour
{
    [SerializeField] GameObject nicknameWindow; // 닉네임 입력 UI 창
    [SerializeField] Button nicknameButton; // 닉네임 확정(캐릭터 생성) 버튼
    [SerializeField] Button closeButton; // 닉네임 창 닫기 버튼

    [Header("Nickname Input")] // UI 인스펙터에서 닉네임 입력 섹션 헤더
    [SerializeField] TMP_InputField nicknameInputField; // 닉네임을 입력받을 TMP_InputField 컴포넌트

    void Start() // Unity의 Start 메서드 - 객체가 활성화될 때 한 번 실행
    {
        if (nicknameButton != null) // 닉네임 확정 버튼이 존재하면
            nicknameButton.onClick.AddListener(CreateCharacters); // 클릭 시 CreateCharacters 메서드 호출 리스너 추가

        if (closeButton != null) // 닫기 버튼이 존재하면
            closeButton.onClick.AddListener(CloseNickname); // 클릭 시 CloseNickname 메서드 호출 리스너 추가
    }

    private void CreateCharacters() // 캐릭터 생성 버튼 클릭 시 호출되는 메서드
    {
        if (nicknameInputField == null || string.IsNullOrEmpty(nicknameInputField.text.Trim())) // 닉네임 입력 필드가 비어있거나 공백만 있는 경우
        {
            Debug.LogWarning("닉네임을 입력해주세요!"); // 경고 로그 출력
            return; // 메서드 종료
        }

        JobData selectedJob = CharacterCreateManager.Instance?.GetSelectedJobData(); // CharacterCreateManager에서 현재 선택된 직업 데이터 가져오기
        if (selectedJob == null) // 선택된 직업이 없는 경우
        {
            Debug.LogWarning("직업이 선택되지 않았습니다!"); // 경고 로그 출력
            return; // 메서드 종료
        }

        string characterName = nicknameInputField.text.Trim(); // 입력된 닉네임 가져오기
        GameDataSaveLoadManager.Instance.CreateCharacter(characterName, selectedJob); // 게임 데이터 매니저를 통해 새 캐릭터 생성 및 저장

        Debug.Log($"캐릭터 생성 완료: {characterName}, 직업: {selectedJob.jobName}"); // 생성 완료 로그 출력

        RefreshCharacterUI(); // 캐릭터 UI를 새로고침하는 메서드 호출
        CloseNickname(); // 닉네임 입력 창 닫기
        ResetCharacterCreation(); // 캐릭터 생성 관련 UI 초기화
    }

    private void RefreshCharacterUI() // 캐릭터 UI를 새로고침하고 마지막 캐릭터를 선택하는 메서드
    {
        // 씬에서 CharacterInfoToggles 인스턴스를 찾음 (비활성화된 오브젝트도 포함하여 찾기)
        CharacterInfoToggles characterInfoToggles = FindAnyObjectByType<CharacterInfoToggles>(FindObjectsInactive.Include);

        if (characterInfoToggles != null) // CharacterInfoToggles 인스턴스를 찾은 경우
        {
            characterInfoToggles.RefreshCharacterData(); // 캐릭터 데이터를 새로고침하여 UI 업데이트

            int lastIndex = GameDataSaveLoadManager.Instance.GameData.characters.Count - 1; // 새로 생성된 캐릭터의 인덱스 (마지막 인덱스)
            if (lastIndex >= 0) // 유효한 인덱스인 경우
            {
                characterInfoToggles.SelectCharacterByIndex(lastIndex); // 새로 생성된(마지막) 캐릭터를 자동으로 선택
            }

            Debug.Log("캐릭터 UI 업데이트 완료"); // 업데이트 완료 로그 출력
        }
        else // CharacterInfoToggles 인스턴스를 찾지 못한 경우
        {
            Debug.LogWarning("CharacterInfoToggles를 찾을 수 없습니다!"); // 경고 로그 출력
        }
    }

    private void CloseNickname() // 닉네임 입력 창을 닫는 메서드
    {
        nicknameWindow.SetActive(false); // 닉네임 창 비활성화
    }

    private void ResetCharacterCreation() // 캐릭터 생성 관련 UI를 초기화하는 메서드
    {
        if (nicknameInputField != null) // 닉네임 입력 필드가 존재하면
            nicknameInputField.text = ""; // 입력 필드의 텍스트를 비움
    }
}