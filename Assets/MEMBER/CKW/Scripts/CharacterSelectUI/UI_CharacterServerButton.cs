// UI_CharacterServerButton.cs (삭제 확인 창 추가)
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameSave;
using System.Collections; // ForceUIUpdate 코루틴을 위해 필요

public class UI_CharacterServerButton : MonoBehaviour
{
    [Header("Button Settings")] // UI 인스펙터에서 버튼 설정 섹션 헤더
    [SerializeField] Button startButton; // 게임 시작 버튼
    [SerializeField] Button exitButton; // 게임 종료 버튼
    [SerializeField] Button serverChangeButton; // 서버 변경 버튼
    [SerializeField] Button characterCreationButton; // 캐릭터 생성 버튼
    [SerializeField] Button characterDeleteButton; // 캐릭터 삭제 버튼

    [Header("Delete Confirmation Panel")] // 삭제 확인 창 설정
    [SerializeField] GameObject deletePanel; // 삭제 확인 패널
    [SerializeField] Button deleteConfirmButton; // 삭제 확인 버튼
    [SerializeField] Button deleteCancelButton; // 삭제 취소 버튼

    [SerializeField] GameObject SelectCharacterWindow; // 캐릭터 선택 창 오브젝트
    [SerializeField] GameObject characterSelectionWindow; // 캐릭터 생성/선택 UI 창 (이름이 유사하여 확인 필요)

    [SerializeField] CharacterInfoToggles characterManager; // CharacterInfoToggles 스크립트 참조

    private CharacterData selectedCharacterData; // 현재 선택된 캐릭터 데이터
    private int selectedCharacterIndex; // 현재 선택된 캐릭터 인덱스

    void Start() // Unity의 Start 메서드 - 객체가 활성화될 때 한 번 실행
    {
        if (startButton != null) // 시작 버튼이 존재하면
            startButton.onClick.AddListener(StartGame); // 클릭 시 StartGame 메서드 호출 리스너 추가

        if (exitButton != null) // 종료 버튼이 존재하면
            exitButton.onClick.AddListener(ExitGame); // 클릭 시 ExitGame 메서드 호출 리스너 추가

        if (serverChangeButton != null) // 서버 변경 버튼이 존재하면
            serverChangeButton.onClick.AddListener(ServerChange); // 클릭 시 ServerChange 메서드 호출 리스너 추가

        if (characterCreationButton != null) // 캐릭터 생성 버튼이 존재하면
            characterCreationButton.onClick.AddListener(CharacterCreate); // 클릭 시 CharacterCreate 메서드 호출 리스너 추가

        if (characterDeleteButton != null) // 캐릭터 삭제 버튼이 존재하면
            characterDeleteButton.onClick.AddListener(ShowDeleteConfirmation); // 클릭 시 삭제 확인 창 표시

        // 삭제 확인 창 버튼들
        if (deleteConfirmButton != null) // 삭제 확인 버튼이 존재하면
            deleteConfirmButton.onClick.AddListener(ConfirmCharacterDelete); // 클릭 시 실제 삭제 실행

        if (deleteCancelButton != null) // 삭제 취소 버튼이 존재하면
            deleteCancelButton.onClick.AddListener(CancelCharacterDelete); // 클릭 시 삭제 취소
    }

    void OnEnable() // Unity의 OnEnable 메서드 - 객체가 활성화될 때 실행
    {
        CharacterInfoToggles.OnCharacterSelected += OnCharacterSelectedHandler; // 캐릭터 선택 이벤트에 핸들러 구독
    }

    void OnDisable() // Unity의 OnDisable 메서드 - 객체가 비활성화될 때 실행
    {
        CharacterInfoToggles.OnCharacterSelected -= OnCharacterSelectedHandler; // 캐릭터 선택 이벤트에서 핸들러 구독 해제
    }

    private void OnCharacterSelectedHandler(CharacterData character, int index) // 캐릭터 선택 이벤트 발생 시 호출되는 핸들러
    {
        selectedCharacterData = character; // 선택된 캐릭터 데이터 저장
        selectedCharacterIndex = index; // 선택된 캐릭터 인덱스 저장
    }

    private void StartGame() // 게임 시작 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
        selectedCharacterData = CharacterInfoToggles.GetCurrentSelectedCharacter();
        Debug.Log($"selectedCharacterData: {(selectedCharacterData != null ? selectedCharacterData.playerSaveData.characterName : "NULL")}");

        if (selectedCharacterData != null)
        {
            var characterToggles = FindAnyObjectByType<CharacterInfoToggles>();
            int currentIndex = characterToggles?.GetSelectedCharacterIndex() ?? -1;

            Debug.Log($"현재 선택된 캐릭터 인덱스: {currentIndex}");
            Debug.Log($"선택된 캐릭터 이름: {selectedCharacterData.playerSaveData.characterName}");

            GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(currentIndex);
            Debug.Log("GameManager.LoadGame() 호출 전");

            // 이부분에서 씬 이동
            SceneLoader.LoadSceneAsync("Field");
        }
        else
        {
            Debug.LogWarning("선택된 캐릭터가 없습니다!");
        }
    }

    private void ExitGame() // 게임 종료 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
#if UNITY_EDITOR // Unity 에디터에서 실행 중인 경우
        EditorApplication.isPlaying = false; // 에디터 플레이 모드 종료
#else // 빌드된 애플리케이션인 경우
        Application.Quit(); // 애플리케이션 종료
#endif
    }

    private void ServerChange() // 서버 변경 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
        SceneManager.LoadScene("CKW_TitleScene"); // "CKW_TitleScene" 씬 로드
    }

    private void CharacterCreate() // 캐릭터 생성 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
        SelectCharacterWindow.SetActive(false); // 캐릭터 선택 창 비활성화
        characterSelectionWindow.SetActive(true); // 캐릭터 생성/선택 창 활성화
    }

    // ⭐ 삭제 확인 창 표시 (기존 CharacterDelete 메서드 대체)
    private void ShowDeleteConfirmation() // 캐릭터 삭제 버튼 클릭 시 확인 창 표시
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
        if (selectedCharacterData == null) // 삭제할 캐릭터가 선택되지 않은 경우
        {
            Debug.LogWarning("삭제할 캐릭터가 선택되지 않았습니다!"); // 경고 로그 출력
            return; // 메서드 종료
        }

        if (deletePanel != null) // 삭제 확인 패널이 존재하는 경우
        {
            deletePanel.SetActive(true); // 삭제 확인 패널 활성화
            Debug.Log($"캐릭터 '{selectedCharacterData.playerSaveData.characterName}' 삭제 확인 창 표시");
        }
    }

    // ⭐ 삭제 확인 - 실제 캐릭터 삭제 실행
    private void ConfirmCharacterDelete() // 삭제 확인 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
        if (selectedCharacterData == null) // 삭제할 캐릭터가 없는 경우
        {
            Debug.LogWarning("삭제할 캐릭터 데이터가 없습니다!");
            CancelCharacterDelete(); // 취소 처리
            return;
        }

        if (characterManager != null) // 캐릭터 매니저가 존재하는 경우
        {
            var characterToDelete = selectedCharacterData; // 삭제할 캐릭터 데이터 변수에 저장
            int indexToDelete = selectedCharacterIndex; // 삭제할 캐릭터 인덱스 저장

            Debug.Log($"캐릭터 '{characterToDelete.playerSaveData.characterName}' 삭제 실행");

            characterManager.DeleteSpecificCharacter(characterToDelete, indexToDelete); // 캐릭터 매니저의 삭제 메서드 호출

            selectedCharacterData = null; // 선택된 캐릭터 데이터 초기화
            selectedCharacterIndex = -1; // 선택된 캐릭터 인덱스 초기화

            StartCoroutine(ForceUIUpdate()); // UI 강제 업데이트 코루틴 시작
        }

        // 삭제 확인 패널 비활성화
        if (deletePanel != null)
        {
            deletePanel.SetActive(false);
        }
    }

    // ⭐ 삭제 취소 - 확인 창 닫기
    private void CancelCharacterDelete() // 삭제 취소 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
        if (deletePanel != null) // 삭제 확인 패널이 존재하는 경우
        {
            deletePanel.SetActive(false); // 삭제 확인 패널 비활성화
            Debug.Log("캐릭터 삭제가 취소되었습니다.");
        }
    }

    private IEnumerator ForceUIUpdate() // UI를 강제로 업데이트하는 코루틴
    {
        yield return null; // 다음 프레임까지 대기
        if (characterManager != null) // 캐릭터 매니저가 존재하는 경우
        {
            characterManager.ForceRefresh(); // 캐릭터 매니저의 UI 새로고침 메서드 호출
        }
    }
}