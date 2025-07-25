// UI_CharacterServerButton.cs (맨 처음 상태 추정)
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
            characterDeleteButton.onClick.AddListener(CharacterDelete); // 클릭 시 CharacterDelete 메서드 호출 리스너 추가
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
        selectedCharacterData = CharacterInfoToggles.GetCurrentSelectedCharacter(); // 현재 선택된 캐릭터 데이터 가져오기

        if (selectedCharacterData != null) // 선택된 캐릭터가 있는 경우
        {
            GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(selectedCharacterIndex); // 게임 데이터 매니저에 선택된 캐릭터 인덱스 저장
            SceneManager.LoadScene("Field"); // "Field" 씬 로드
        }
        else // 선택된 캐릭터가 없는 경우
        {
            Debug.LogWarning("선택된 캐릭터가 없습니다!"); // 경고 로그 출력
        }
    }


    private void ExitGame() // 게임 종료 버튼 클릭 시 호출되는 메서드
    {
#if UNITY_EDITOR // Unity 에디터에서 실행 중인 경우
        EditorApplication.isPlaying = false; // 에디터 플레이 모드 종료
#else // 빌드된 애플리케이션인 경우
        Application.Quit(); // 애플리케이션 종료
#endif
    }


    private void ServerChange() // 서버 변경 버튼 클릭 시 호출되는 메서드
    {
        SceneManager.LoadScene("CKW_TitleScene"); // "CKW_TitleScene" 씬 로드
    }


    private void CharacterCreate() // 캐릭터 생성 버튼 클릭 시 호출되는 메서드
    {
        SelectCharacterWindow.SetActive(false); // 캐릭터 선택 창 비활성화
        characterSelectionWindow.SetActive(true); // 캐릭터 생성/선택 창 활성화
    }


    private void CharacterDelete() // 캐릭터 삭제 버튼 클릭 시 호출되는 메서드
    {
        if (selectedCharacterData == null) // 삭제할 캐릭터가 선택되지 않은 경우
        {
            Debug.LogWarning("삭제할 캐릭터가 선택되지 않았습니다!"); // 경고 로그 출력
            return; // 메서드 종료
        }

        if (characterManager != null) // 캐릭터 매니저가 존재하는 경우
        {
            var characterToDelete = selectedCharacterData; // 삭제할 캐릭터 데이터 변수에 저장

            characterManager.DeleteSpecificCharacter(characterToDelete, selectedCharacterIndex); // 캐릭터 매니저의 삭제 메서드 호출

            selectedCharacterData = null; // 선택된 캐릭터 데이터 초기화
            selectedCharacterIndex = -1; // 선택된 캐릭터 인덱스 초기화

            StartCoroutine(ForceUIUpdate()); // UI 강제 업데이트 코루틴 시작
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