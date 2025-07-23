// CharacterInfoToggles.cs (맨 처음 상태 추정)
using System.Collections.Generic;
using UnityEngine;
using GameSave;
using UnityEngine.UI;
using System;
using TMPro;
// using System.Collections; // 코루틴이 없었다면 이 using도 없었을 것입니다.

/// 캐릭터 정보를 토글 형태로 표시하고 선택 기능을 제공하는 클래스
public class CharacterInfoToggles : MonoBehaviour
{
    [Header("토글 프리팹 설정")]
    [SerializeField] private GameObject characterTogglePrefab;
    [SerializeField] private Transform toggleParent;
    [SerializeField] private ToggleGroup toggleGroup;

    [Header("선택된 캐릭터 정보 표시 (선택사항)")]
    // 이 필드들은 외부에서 설정되어야 합니다.
    public GameObject selectedCharacterPanel;
    public TextMeshProUGUI selectedCharacterName;
    public TextMeshProUGUI selectedCharacterClass;

    private List<CharacterData> characterDatas = new List<CharacterData>();
    private List<CharacterToggleUI> createdToggleUIs = new List<CharacterToggleUI>();
    private Dictionary<Toggle, CharacterData> toggleToCharacterMap = new Dictionary<Toggle, CharacterData>();
    private Dictionary<Toggle, int> toggleToIndexMap = new Dictionary<Toggle, int>();
    private CharacterData selectedCharacter;
    private int selectedCharacterIndex = -1;

    public static event Action<CharacterData, int> OnCharacterSelected;

    void Start()
    {
        RefreshCharacterData();
        Unvoid(); // 이 함수가 원래 있었다면 그대로 둠.
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제 (아마 비어있었을 것입니다)
    }

    void Unvoid()
    {
        // 비어있는 함수였을 것입니다.
    }

    private void LoadCharacterData()
    {
        // ⭐ GameDataSaveLoadManager.Instance.GameData.characters를 직접 참조
        characterDatas = GameDataSaveLoadManager.Instance.GameData.characters;
        // Debug.Log($"캐릭터 데이터 로드 완료: {characterDatas.Count}개"); // 이 로그도 없었을 수 있습니다.
    }

    private void CreateToggles()
    {
        ClearExistingToggles();

        for (int i = 0; i < characterDatas.Count; i++)
        {
            CreateToggle(i);
        }
    }

    private void ClearExistingToggles()
    {
        foreach (var toggleUI in createdToggleUIs)
        {
            if (toggleUI != null && toggleUI.toggle != null)
            {
                Destroy(toggleUI.toggle.gameObject); // DestroyImmediate였을 가능성도 있습니다.
            }
        }

        createdToggleUIs.Clear();
        toggleToCharacterMap.Clear();
        toggleToIndexMap.Clear();
    }

    private void CreateToggle(int index)
    {
        CharacterData character = characterDatas[index];

        GameObject toggleObject = Instantiate(characterTogglePrefab, toggleParent);
        CharacterToggleUI toggleUI = toggleObject.GetComponent<CharacterToggleUI>();

        if (toggleUI == null)
        {
            Debug.LogError($"프리팹에 CharacterToggleUI 컴포넌트가 없습니다! 인덱스: {index}");
            return;
        }

        if (toggleGroup != null)
        {
            toggleUI.toggle.group = toggleGroup;
        }

        toggleToCharacterMap[toggleUI.toggle] = character;
        toggleToIndexMap[toggleUI.toggle] = index;

        createdToggleUIs.Add(toggleUI);
        UpdateToggleUI(toggleUI, character);

        int capturedIndex = index;
        CharacterData capturedCharacter = character; // 아마 이것도 없었을 수 있음 (클로저 문제 유발)

        toggleUI.toggle.onValueChanged.RemoveAllListeners(); // 이것도 없었을 수 있음
        toggleUI.toggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                OnToggleValueChanged(toggleUI.toggle, capturedCharacter, capturedIndex);
            }
        });

        // Debug.Log($"토글 {index} 생성 완료: {character.characterName}"); // 이 로그도 없었을 수 있습니다.
    }

    private void UpdateToggleUI(CharacterToggleUI toggleUI, CharacterData character)
    {
        if (toggleUI.characterNameText != null)
            toggleUI.characterNameText.text = $"Lv. {character.playerSaveData.level} {character.playerSaveData.characterName}";

        if (toggleUI.characterClassText != null)
            toggleUI.characterClassText.text = $"{character.playerSaveData.maxHealth}"; // class text에 레벨 표시
    }

    private void OnToggleValueChanged(Toggle toggle, CharacterData character, int index)
    {
        selectedCharacter = character;
        selectedCharacterIndex = index;

        // Debug.Log($"캐릭터 선택됨: {character.characterName} (인덱스: {index})"); // 이 로그도 없었을 수 있습니다.

        UpdateSelectedCharacterInfo();
        GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(index);
        OnCharacterSelected?.Invoke(character, index);
    }

    private void UpdateSelectedCharacterInfo()
    {
        if (selectedCharacter == null) return; // selectedCharacterPanel null 체크 추가해야 합니다.

        if (selectedCharacterPanel != null)
            selectedCharacterPanel.SetActive(true);

        if (selectedCharacterName != null)
            selectedCharacterName.text = selectedCharacter.playerSaveData.characterName;
        if (selectedCharacterClass != null)
            selectedCharacterClass.text = $"레벨: {selectedCharacter.playerSaveData.level}";
    }

    public CharacterData GetSelectedCharacter()
    {
        return selectedCharacter;
    }

    public int GetSelectedCharacterIndex()
    {
        return selectedCharacterIndex;
    }

    public void SelectCharacterByIndex(int index)
    {
        if (index >= 0 && index < createdToggleUIs.Count && createdToggleUIs[index].toggle != null)
        {
            createdToggleUIs[index].toggle.isOn = true;
        }
    }

    public static CharacterData GetCurrentSelectedCharacter()
    {
        CharacterInfoToggles instance = FindAnyObjectByType<CharacterInfoToggles>();
        return instance?.GetSelectedCharacter();
    }

    public void RefreshCharacterData()
    {
        LoadCharacterData();
        CreateToggles();
    }

    public void AddNewCharacter(CharacterData newCharacter)
    {
        characterDatas.Add(newCharacter); // 이 줄이 있었을 수 있습니다.
        GameDataSaveLoadManager.Instance.GameData.characters.Add(newCharacter);
        GameDataSaveLoadManager.Instance.SaveGame();
        RefreshCharacterData();
    }

    // DeleteSpecificCharacter 메서드는 제가 이전 대화에서 여러 번 수정 제안을 드렸으므로,
    // "맨 처음" 상태는 매우 단순했을 것으로 추정됩니다.
    public void DeleteSpecificCharacter(CharacterData targetCharacter, int index)
    {
        if (targetCharacter == null)
        {
            Debug.LogWarning("삭제할 캐릭터가 null입니다!");
            return;
        }

        // ⭐ 맨 처음에는 characterID 기반 검색 로직이 없었을 가능성이 높습니다.
        // 그리고 try-catch 블록도 없었을 수 있습니다.
        // realIndex는 아마 인자로 받은 index를 그대로 사용했을 것입니다.

        // LoadCharacterData(); // 이 호출도 없었을 수 있음

        // int realIndex = characterDatas.IndexOf(targetCharacter); // characterID Equals 없으면 이 방식은 안됨.
        // 또는 그냥 인덱스 사용:
        int realIndex = index; // ⭐ 맨 처음에는 이렇게 인자로 받은 인덱스를 직접 사용했을 가능성.

        if (realIndex < 0 || realIndex >= characterDatas.Count)
        {
            Debug.LogError($"[CharInfoToggles] 삭제할 캐릭터 인덱스가 유효하지 않습니다! 인덱스: {realIndex}, 현재 캐릭터 수: {characterDatas.Count}");
            return;
        }

        // ⭐ 삭제 실행 (characterDatas와 GameData.characters 모두에서 제거)
        characterDatas.RemoveAt(realIndex);
        GameDataSaveLoadManager.Instance.GameData.characters.RemoveAt(realIndex);
        GameDataSaveLoadManager.Instance.SaveGame();

        // Debug.Log("캐릭터 데이터 삭제 완료"); // 이 로그도 없었을 수 있습니다.

        // ⭐ 선택된 캐릭터 관련 로직도 단순했거나 없었을 수 있습니다.
        if (selectedCharacterIndex == realIndex) // 또는 selectedCharacter.Equals(targetCharacter)
        {
            selectedCharacter = null;
            selectedCharacterIndex = -1;
            GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(-1);
        }
        // 인덱스 조정 로직도 없었을 수 있습니다.

        RefreshCharacterData(); // 이 시점에서 UI 갱신

        // ⭐ 남은 캐릭터가 있다면 첫 번째 캐릭터 자동 선택 (이 부분은 기존에도 있었을 수 있습니다.)
        if (characterDatas.Count > 0)
        {
            SelectCharacterByIndex(0);
        }
        else
        {
            if (selectedCharacterPanel != null) selectedCharacterPanel.SetActive(false);
            UpdateSelectedCharacterInfo();
        }
    }

    public void ForceRefresh()
    {
        RefreshCharacterData();
    }
}