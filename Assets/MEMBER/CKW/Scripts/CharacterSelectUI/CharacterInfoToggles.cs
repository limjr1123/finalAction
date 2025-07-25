// CharacterInfoToggles.cs - 삭제 문제 수정
using System.Collections.Generic;
using UnityEngine;
using GameSave;
using UnityEngine.UI;
using System;
using TMPro;

public class CharacterInfoToggles : MonoBehaviour
{
    [Header("토글 프리팹 설정")] // UI 인스펙터에서 토글 프리팹 설정 섹션 헤더
    [SerializeField] private GameObject characterTogglePrefab; // 캐릭터 토글을 생성할 때 사용할 프리팹
    [SerializeField] private Transform toggleParent; // 생성된 토글들의 부모 Transform
    [SerializeField] private ToggleGroup toggleGroup; // 토글들을 그룹화하여 하나만 선택되도록 하는 그룹

    [Header("선택된 캐릭터 정보 표시")] // UI 인스펙터에서 선택된 캐릭터 정보 표시 섹션 헤더
    public GameObject selectedCharacterPanel; // 선택된 캐릭터 정보를 표시할 패널
    public TextMeshProUGUI selectedCharacterName; // 선택된 캐릭터의 이름을 표시할 텍스트
    public TextMeshProUGUI selectedCharacterClass; // 선택된 캐릭터의 클래스를 표시할 텍스트

    private List<CharacterData> characterDatas = new List<CharacterData>(); // 캐릭터 데이터들을 저장하는 리스트
    private List<CharacterToggleUI> createdToggleUIs = new List<CharacterToggleUI>(); // 생성된 토글 UI들을 저장하는 리스트
    private Dictionary<Toggle, CharacterData> toggleToCharacterMap = new Dictionary<Toggle, CharacterData>(); // 토글과 캐릭터 데이터를 매핑하는 딕셔너리
    private Dictionary<Toggle, int> toggleToIndexMap = new Dictionary<Toggle, int>(); // 토글과 인덱스를 매핑하는 딕셔너리
    private CharacterData selectedCharacter; // 현재 선택된 캐릭터 데이터
    private int selectedCharacterIndex = -1; // 현재 선택된 캐릭터의 인덱스

    public static event Action<CharacterData, int> OnCharacterSelected; // 캐릭터가 선택되었을 때 발생하는 정적 이벤트

    void Start() // Unity의 Start 메서드 - 객체가 활성화될 때 한 번 실행
    {
        RefreshCharacterData(); // 캐릭터 데이터를 새로고침하고 UI를 생성
    }


    void OnDestroy() // Unity의 OnDestroy 메서드 - 객체가 파괴될 때 실행
    {
        // 이벤트 구독 해제 (현재는 비어있음)
    }


    private void LoadCharacterData() // 게임 데이터에서 캐릭터 데이터를 로드하는 메서드
    {
        // ⭐ 참조가 아닌 새로운 리스트로 복사하여 중복 삭제 문제 해결
        characterDatas.Clear(); // 기존 캐릭터 데이터 리스트를 비움
        characterDatas.AddRange(GameDataSaveLoadManager.Instance.GameData.characters); // 게임 데이터의 캐릭터들을 복사하여 추가
    }


    private void CreateToggles() // 캐릭터 데이터를 기반으로 토글 UI들을 생성하는 메서드
    {
        ClearExistingToggles(); // 기존 토글들을 모두 제거

        for (int i = 0; i < characterDatas.Count; i++) // 캐릭터 데이터 개수만큼 반복
        {
            CreateToggle(i); // 각 인덱스에 대해 토글을 생성
        }
    }


    private void ClearExistingToggles() // 기존에 생성된 모든 토글들을 제거하는 메서드
    {
        foreach (var toggleUI in createdToggleUIs) // 생성된 토글 UI들을 순회
        {
            if (toggleUI != null && toggleUI.toggle != null) // 토글 UI와 토글이 null이 아닌 경우
            {
                Destroy(toggleUI.toggle.gameObject); // 토글 게임오브젝트를 파괴
            }
        }

        createdToggleUIs.Clear(); // 생성된 토글 UI 리스트를 비움
        toggleToCharacterMap.Clear(); // 토글-캐릭터 매핑 딕셔너리를 비움
        toggleToIndexMap.Clear(); // 토글-인덱스 매핑 딕셔너리를 비움
    }


    private void CreateToggle(int index) // 특정 인덱스의 캐릭터에 대한 토글을 생성하는 메서드
    {
        CharacterData character = characterDatas[index]; // 해당 인덱스의 캐릭터 데이터를 가져옴

        GameObject toggleObject = Instantiate(characterTogglePrefab, toggleParent); // 토글 프리팹을 인스턴스화하여 부모 하위에 생성
        CharacterToggleUI toggleUI = toggleObject.GetComponent<CharacterToggleUI>(); // 생성된 토글 오브젝트에서 CharacterToggleUI 컴포넌트를 가져옴

        if (toggleUI == null) // 토글 UI 컴포넌트가 없는 경우
        {
            Debug.LogError($"프리팹에 CharacterToggleUI 컴포넌트가 없습니다! 인덱스: {index}"); // 에러 로그 출력
            return; // 메서드 종료
        }

        if (toggleGroup != null) // 토글 그룹이 설정되어 있는 경우
        {
            toggleUI.toggle.group = toggleGroup; // 토글을 토글 그룹에 추가
        }

        toggleToCharacterMap[toggleUI.toggle] = character; // 토글과 캐릭터 데이터를 매핑
        toggleToIndexMap[toggleUI.toggle] = index; // 토글과 인덱스를 매핑

        createdToggleUIs.Add(toggleUI); // 생성된 토글 UI를 리스트에 추가
        UpdateToggleUI(toggleUI, character); // 토글 UI의 내용을 캐릭터 데이터로 업데이트

        int capturedIndex = index; // 클로저에서 사용할 인덱스를 캡처
        CharacterData capturedCharacter = character; // 클로저에서 사용할 캐릭터 데이터를 캡처

        toggleUI.toggle.onValueChanged.RemoveAllListeners(); // 토글의 기존 이벤트 리스너들을 모두 제거
        toggleUI.toggle.onValueChanged.AddListener((isOn) => // 토글 값 변경 이벤트에 리스너 추가
        {
            if (isOn) // 토글이 켜진 경우
            {
                OnToggleValueChanged(toggleUI.toggle, capturedCharacter, capturedIndex); // 토글 값 변경 처리 메서드 호출
            }
        });
    }


    private void UpdateToggleUI(CharacterToggleUI toggleUI, CharacterData character) // 각 토글 UI를 캐릭터 데이터와 동기화함
    {
        if (toggleUI.characterNameText != null) // 캐릭터 이름 텍스트가 있는 경우
            toggleUI.characterNameText.text = $"Lv. {character.playerSaveData.level} {character.playerSaveData.characterName}"; // 레벨과 이름을 표시

        if (character.playerSaveData.jobData != null) // 캐릭터의 직업 데이터가 있는 경우
            Debug.Log("qweqwe"); // 디버그 로그 출력 (테스트용)

        if (toggleUI.characterClassText != null) // 캐릭터 클래스 텍스트가 있는 경우
            toggleUI.characterClassText.text = $"{character.playerSaveData.jobData.jobName}"; // 직업 이름을 표시

        // ⭐ 직업 이미지 설정 추가
        if (toggleUI.characterImage != null && character.playerSaveData.jobData != null) // 캐릭터 이미지와 직업 데이터가 있는 경우
        {
            if (character.playerSaveData.jobData.jobIcon != null) // 직업 아이콘이 있는 경우
            {
                toggleUI.characterImage.sprite = character.playerSaveData.jobData.jobIcon; // 토글 UI의 이미지를 직업 아이콘으로 설정
            }
            else // 직업 아이콘이 없는 경우
            {
                Debug.LogWarning($"캐릭터 {character.playerSaveData.characterName}의 직업 이미지가 없습니다!"); // 경고 로그 출력
            }
        }
    }


    private void OnToggleValueChanged(Toggle toggle, CharacterData character, int index) // 토글 값이 변경되었을 때 호출되는 메서드
    {
        selectedCharacter = character; // 선택된 캐릭터를 설정
        selectedCharacterIndex = index; // 선택된 캐릭터 인덱스를 설정

        UpdateSelectedCharacterInfo(); // 선택된 캐릭터 정보 UI를 업데이트
        GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(index); // 게임 데이터 매니저에 선택된 캐릭터 인덱스를 저장
        OnCharacterSelected?.Invoke(character, index); // 캐릭터 선택 이벤트를 발생시킴
    }


    private void UpdateSelectedCharacterInfo() // 선택된 캐릭터 정보 UI를 업데이트하는 메서드
    {
        if (selectedCharacter == null) return; // 선택된 캐릭터가 없으면 메서드 종료

        if (selectedCharacterPanel != null) // 선택된 캐릭터 패널이 있는 경우
            selectedCharacterPanel.SetActive(true); // 패널을 활성화

        if (selectedCharacterName != null) // 선택된 캐릭터 이름 텍스트가 있는 경우
            selectedCharacterName.text = selectedCharacter.playerSaveData.characterName; // 캐릭터 이름을 표시
        if (selectedCharacterClass != null) // 선택된 캐릭터 클래스 텍스트가 있는 경우
            selectedCharacterClass.text = $"레벨: {selectedCharacter.playerSaveData.level}"; // 캐릭터 레벨을 표시
    }


    public CharacterData GetSelectedCharacter() // 현재 선택된 캐릭터 데이터를 반환하는 메서드
    {
        return selectedCharacter; // 선택된 캐릭터 반환
    }


    public int GetSelectedCharacterIndex() // 현재 선택된 캐릭터의 인덱스를 반환하는 메서드
    {
        return selectedCharacterIndex; // 선택된 캐릭터 인덱스 반환
    }


    public void SelectCharacterByIndex(int index) // 특정 인덱스의 캐릭터를 선택하는 메서드
    {
        if (index >= 0 && index < createdToggleUIs.Count && createdToggleUIs[index].toggle != null) // 인덱스가 유효하고 해당 토글이 존재하는 경우
        {
            createdToggleUIs[index].toggle.isOn = true; // 해당 토글을 켬
        }
    }


    public static CharacterData GetCurrentSelectedCharacter() // 현재 선택된 캐릭터를 가져오는 정적 메서드
    {
        CharacterInfoToggles instance = FindAnyObjectByType<CharacterInfoToggles>(); // 씬에서 CharacterInfoToggles 인스턴스를 찾음
        return instance?.GetSelectedCharacter(); // 인스턴스가 있으면 선택된 캐릭터를 반환, 없으면 null 반환
    }


    public void RefreshCharacterData() // 캐릭터 데이터를 새로고침하고 UI를 재생성하는 메서드
    {
        LoadCharacterData(); // 캐릭터 데이터를 로드
        CreateToggles(); // 토글들을 생성
    }



    // ⭐ 삭제 함수 수정 - 중복 삭제 문제 해결
    public void DeleteSpecificCharacter(CharacterData targetCharacter, int index) // 특정 캐릭터를 삭제하는 메서드
    {
        if (targetCharacter == null) // 삭제할 캐릭터가 null인 경우
        {
            Debug.LogWarning("삭제할 캐릭터가 null입니다!"); // 경고 로그 출력
            return; // 메서드 종료
        }

        // 최신 데이터로 다시 로드
        LoadCharacterData(); // 캐릭터 데이터를 다시 로드

        // 유효성 검사
        if (index < 0 || index >= GameDataSaveLoadManager.Instance.GameData.characters.Count) // 인덱스가 유효하지 않은 경우
        {
            Debug.LogError($"삭제할 캐릭터 인덱스가 유효하지 않습니다! 인덱스: {index}"); // 에러 로그 출력
            return; // 메서드 종료
        }

        // ⭐ GameData.characters에서만 삭제 (characterDatas는 다음 RefreshCharacterData에서 자동 갱신됨)
        GameDataSaveLoadManager.Instance.GameData.characters.RemoveAt(index); // 게임 데이터에서 해당 인덱스의 캐릭터를 삭제
        GameDataSaveLoadManager.Instance.SaveGameDataToJason(); // 게임 데이터를 JSON으로 저장

        Debug.Log($"캐릭터 삭제 완료. 남은 캐릭터 수: {GameDataSaveLoadManager.Instance.GameData.characters.Count}"); // 삭제 완료 로그 출력

        // 선택된 캐릭터 관련 처리
        if (selectedCharacterIndex == index) // 삭제된 캐릭터가 현재 선택된 캐릭터인 경우
        {
            selectedCharacter = null; // 선택된 캐릭터를 null로 설정
            selectedCharacterIndex = -1; // 선택된 캐릭터 인덱스를 -1로 설정
            GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(-1); // 게임 데이터 매니저의 선택된 캐릭터 인덱스를 -1로 설정
        }
        else if (selectedCharacterIndex > index) // 선택된 캐릭터가 삭제된 캐릭터보다 뒤에 있는 경우
        {
            // 선택된 캐릭터가 삭제된 캐릭터보다 뒤에 있다면 인덱스 조정
            selectedCharacterIndex--; // 선택된 캐릭터 인덱스를 1 감소
            GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(selectedCharacterIndex); // 게임 데이터 매니저에 조정된 인덱스를 저장
        }

        Debug.Log($"{toggleToIndexMap.Count}");
        // UI 갱신
        RefreshCharacterData(); // 캐릭터 데이터를 새로고침하여 UI 업데이트

        // 남은 캐릭터가 있다면 첫 번째 캐릭터 자동 선택
        if (GameDataSaveLoadManager.Instance.GameData.characters.Count > 0) // 남은 캐릭터가 있는 경우
        {
            SelectCharacterByIndex(0); // 첫 번째 캐릭터를 자동으로 선택
        }
        else // 남은 캐릭터가 없는 경우
        {
            if (selectedCharacterPanel != null) // 선택된 캐릭터 패널이 있는 경우
                selectedCharacterPanel.SetActive(false); // 패널을 비활성화
        }
    }


    public void ForceRefresh() // 강제로 데이터를 새로고침하는 메서드
    {
        RefreshCharacterData(); // 캐릭터 데이터를 새로고침
    }
}