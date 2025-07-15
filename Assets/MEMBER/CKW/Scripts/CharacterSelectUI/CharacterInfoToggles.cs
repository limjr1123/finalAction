using System.Collections.Generic;
using UnityEngine;
using GameSave;
using UnityEngine.UI;
using System;
using TMPro;

/// 캐릭터 정보를 토글 형태로 표시하고 선택 기능을 제공하는 클래스
public class CharacterInfoToggles : MonoBehaviour
{
    [Header("토글 프리팹 설정")]
    [SerializeField] private GameObject characterTogglePrefab; // 캐릭터 토글을 생성할 때 사용할 프리팹
    [SerializeField] private Transform toggleParent; // 생성된 토글들이 배치될 부모 Transform
    [SerializeField] private ToggleGroup toggleGroup; // 토글들을 그룹으로 관리하여 하나만 선택되도록 하는 ToggleGroup

    [Header("선택된 캐릭터 정보 표시 (선택사항)")]
    private GameObject selectedCharacterPanel; // 선택된 캐릭터 정보를 표시할 패널 (선택사항)
    private TextMeshProUGUI selectedCharacterName; // 선택된 캐릭터의 이름을 표시할 텍스트
    private TextMeshProUGUI selectedCharacterClass; // 선택된 캐릭터의 레벨을 표시할 텍스트


    private List<CharacterData> characterDatas = new List<CharacterData>(); // 게임에서 불러온 캐릭터 데이터 목록
    private List<CharacterToggleUI> createdToggleUIs = new List<CharacterToggleUI>(); // 생성된 토글 UI 컴포넌트들을 관리하는 리스트
    private Dictionary<Toggle, CharacterData> toggleToCharacterMap = new Dictionary<Toggle, CharacterData>(); // 토글과 캐릭터 데이터를 매핑하는 딕셔너리
    private CharacterData selectedCharacter; // 현재 선택된 캐릭터 데이터

    public static event Action<CharacterData, int> OnCharacterSelected; // 캐릭터가 선택될 때 발생하는 이벤트

    /// 게임 오브젝트가 시작될 때 호출되는 메서드
    void Start()
    {
        RefreshCharacterData(); // 캐릭터 데이터를 새로고침하여 토글 생성

        // 캐릭터 데이터 변경 이벤트 구독
        Unvoid();

    }


    /// 게임 오브젝트가 파괴될 때 호출되는 메서드
    void OnDestroy()
    {
        // 이벤트 구독 해제
        //GameDataSaveLoadManager.Instance.OnCharacterDataChanged -= RefreshCharacterData;
    }


    void Unvoid()
    {
        //GameDataSaveLoadManager.Instance.OnCharacterDataChanged += RefreshCharacterData;
    }


    /// 게임 데이터에서 캐릭터 데이터를 불러오는 메서드
    private void LoadCharacterData()
    {
        characterDatas = GameDataSaveLoadManager.Instance.GameData.characters; // 게임 데이터 매니저에서 캐릭터 목록 가져오기
        Debug.Log($"캐릭터 데이터 로드 완료: {characterDatas.Count}개"); // 로드된 캐릭터 수 로그 출력
    }

    /// 캐릭터 데이터를 기반으로 토글들을 생성하는 메서드
    private void CreateToggles()
    {
        // 기존 토글들 제거
        ClearExistingToggles();

        // 캐릭터 데이터 수만큼 토글 생성
        for (int i = 0; i < characterDatas.Count; i++)
        {
            CreateToggle(i); // 각 캐릭터에 대해 토글 생성
        }
    }

    /// 기존에 생성된 토글들을 모두 제거하는 메서드
    private void ClearExistingToggles()
    {
        // 기존 토글들 삭제
        foreach (var toggleUI in createdToggleUIs)
        {
            if (toggleUI != null && toggleUI.toggle != null) // null 체크
            {
                DestroyImmediate(toggleUI.toggle.gameObject); // 토글 게임 오브젝트 즉시 삭제
            }
        }

        createdToggleUIs.Clear(); // 생성된 토글 UI 리스트 초기화
        toggleToCharacterMap.Clear(); // 토글-캐릭터 매핑 딕셔너리 초기화
    }

    /// 특정 인덱스의 캐릭터에 대한 토글을 생성하는 메서드
    private void CreateToggle(int index)
    {
        CharacterData character = characterDatas[index]; // 해당 인덱스의 캐릭터 데이터 가져오기

        // 프리팹에서 토글 생성
        GameObject toggleObject = Instantiate(characterTogglePrefab, toggleParent); // 프리팹을 부모 Transform 하위에 인스턴스화

        // 토글 UI 컴포넌트 가져오기
        CharacterToggleUI toggleUI = toggleObject.GetComponent<CharacterToggleUI>(); // 생성된 오브젝트에서 CharacterToggleUI 컴포넌트 가져오기

        if (toggleUI == null) // 컴포넌트가 없는 경우 에러 처리
        {
            Debug.LogError($"프리팹에 CharacterToggleUI 컴포넌트가 없습니다! 인덱스: {index}");
            return; // 메서드 종료
        }

        // 토글을 ToggleGroup에 연결
        if (toggleGroup != null) // ToggleGroup이 설정되어 있는 경우
        {
            toggleUI.toggle.group = toggleGroup; // 토글을 그룹에 추가하여 하나만 선택되도록 설정
        }

        // 토글과 캐릭터 데이터 매핑
        toggleToCharacterMap[toggleUI.toggle] = character; // 딕셔너리에 토글과 캐릭터 데이터 연결

        // 생성된 토글 UI 리스트에 추가
        createdToggleUIs.Add(toggleUI); // 관리 목적으로 리스트에 추가

        // 토글 UI 업데이트
        UpdateToggleUI(toggleUI, character); // 토글의 표시 정보 업데이트

        // 토글 이벤트 연결
        toggleUI.toggle.onValueChanged.RemoveAllListeners(); // 기존 리스너 모두 제거
        toggleUI.toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggleUI.toggle, isOn)); // 토글 상태 변경 이벤트 연결

        Debug.Log($"토글 {index} 생성 완료: {character.characterName}"); // 토글 생성 완료 로그
    }

    /// 토글 UI의 표시 정보를 업데이트하는 메서드
    /// <param name="toggleUI">업데이트할 토글 UI</param>
    /// <param name="character">표시할 캐릭터 데이터</param>
    private void UpdateToggleUI(CharacterToggleUI toggleUI, CharacterData character)
    {
        // 각 토글의 전용 UI 요소들 업데이트
        if (toggleUI.characterNameText != null) // 캐릭터 이름 텍스트가 있는 경우
            toggleUI.characterNameText.text = $"Lv. {character.level} {character.characterName}"; // 레벨과 이름 표시

        if (toggleUI.characterClassText != null) // 캐릭터 클래스 텍스트가 있는 경우
            toggleUI.characterClassText.text = $"{character.level}"; // 레벨 표시 (클래스 대신 레벨 사용)
    }

    /// 토글의 상태가 변경될 때 호출되는 메서드
    /// <param name="toggle">상태가 변경된 토글</param>
    /// <param name="isOn">토글의 현재 상태 (true/false)</param>
    private void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        if (isOn && toggleToCharacterMap.ContainsKey(toggle)) // 토글이 선택되고 매핑된 캐릭터가 있는 경우
        {
            selectedCharacter = toggleToCharacterMap[toggle]; // 선택된 캐릭터 설정
            int characterIndex = characterDatas.IndexOf(selectedCharacter); // 선택된 캐릭터의 인덱스 찾기

            Debug.Log($"캐릭터 선택됨: {selectedCharacter.characterName} (인덱스: {characterIndex})"); // 선택 로그

            // 선택된 캐릭터 정보 업데이트 (별도 패널이 있다면)
            UpdateSelectedCharacterInfo();

            // 게임 데이터에 선택된 캐릭터 인덱스 저장
            GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(characterIndex);

            // 이벤트 발생
            OnCharacterSelected?.Invoke(selectedCharacter, characterIndex); // 캐릭터 선택 이벤트 발생
        }
    }

    /// 선택된 캐릭터의 정보를 UI에 업데이트하는 메서드
    private void UpdateSelectedCharacterInfo()
    {
        if (selectedCharacter == null) return; // 선택된 캐릭터가 없으면 종료

        // 선택된 캐릭터 패널 활성화 (선택사항)
        if (selectedCharacterPanel != null) // 패널이 설정되어 있는 경우
            selectedCharacterPanel.SetActive(true); // 패널 활성화

        // 선택된 캐릭터 정보 표시 (선택사항)
        if (selectedCharacterName != null) // 이름 텍스트가 있는 경우
            selectedCharacterName.text = selectedCharacter.characterName; // 캐릭터 이름 표시
        if (selectedCharacterClass != null) // 레벨 텍스트가 있는 경우
            selectedCharacterClass.text = $"레벨: {selectedCharacter.level}"; // 레벨 표시          // 나중에 직업으로 변경

    }

    /// 현재 선택된 캐릭터 데이터를 반환하는 메서드
    /// <returns>선택된 캐릭터 데이터</returns>
    public CharacterData GetSelectedCharacter()
    {
        return selectedCharacter; // 현재 선택된 캐릭터 반환
    }

    /// 현재 선택된 캐릭터의 인덱스를 반환하는 메서드
    /// <returns>선택된 캐릭터의 인덱스 (선택된 캐릭터가 없으면 -1)</returns>
    public int GetSelectedCharacterIndex()
    {
        return selectedCharacter != null ? characterDatas.IndexOf(selectedCharacter) : -1; // 선택된 캐릭터가 있으면 인덱스 반환, 없으면 -1
    }

    /// 특정 인덱스의 캐릭터를 선택하는 메서드
    /// <param name="index">선택할 캐릭터의 인덱스</param>
    public void SelectCharacterByIndex(int index)
    {
        if (index >= 0 && index < createdToggleUIs.Count && createdToggleUIs[index].toggle != null) // 유효한 인덱스인지 확인
        {
            createdToggleUIs[index].toggle.isOn = true; // 해당 인덱스의 토글 선택
        }
    }

    /// 현재 선택된 캐릭터를 반환하는 정적 메서드
    /// <returns>현재 선택된 캐릭터 데이터</returns>
    public static CharacterData GetCurrentSelectedCharacter()
    {
        // 현재 활성화된 CharacterInfoToggles 인스턴스 찾기
        CharacterInfoToggles instance = FindAnyObjectByType<CharacterInfoToggles>(); // 씬에서 CharacterInfoToggles 인스턴스 찾기
        return instance?.GetSelectedCharacter(); // 찾은 인스턴스에서 선택된 캐릭터 반환 (null이면 null 반환)
    }

    /// 캐릭터 데이터를 새로고침하는 메서드
    public void RefreshCharacterData()
    {
        LoadCharacterData(); // 캐릭터 데이터 로드
        CreateToggles(); // 토글들 생성
    }

    /// 새 캐릭터를 추가하는 메서드
    /// <param name="newCharacter">추가할 새 캐릭터 데이터</param>
    public void AddNewCharacter(CharacterData newCharacter)
    {
        characterDatas.Add(newCharacter); // 로컬 캐릭터 데이터 리스트에 추가
        GameDataSaveLoadManager.Instance.GameData.characters.Add(newCharacter); // 게임 데이터에 추가
        GameDataSaveLoadManager.Instance.SaveGame(); // 게임 데이터 저장

        RefreshCharacterData(); // UI 새로고침
    }

    /// 선택된 캐릭터를 삭제하는 메서드
    public void DeleteSelectedCharacter()
    {
        if (selectedCharacter != null) // 선택된 캐릭터가 있는 경우
        {
            int index = characterDatas.IndexOf(selectedCharacter); // 선택된 캐릭터의 인덱스 찾기
            if (index >= 0) // 유효한 인덱스인 경우
            {
                characterDatas.RemoveAt(index); // 로컬 리스트에서 제거
                GameDataSaveLoadManager.Instance.GameData.characters.RemoveAt(index); // 게임 데이터에서 제거
                GameDataSaveLoadManager.Instance.SaveGame(); // 게임 데이터 저장

                RefreshCharacterData(); // UI 새로고침
            }
        }
    }

    /// 외부에서 수동으로 갱신을 요청할 때 사용하는 메서드
    public void ForceRefresh()
    {
        RefreshCharacterData(); // 캐릭터 데이터 새로고침
    }
}