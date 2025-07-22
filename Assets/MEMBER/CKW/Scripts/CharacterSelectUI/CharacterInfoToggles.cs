// CharacterInfoToggles.cs - 삭제 문제 수정
using System.Collections.Generic;
using UnityEngine;
using GameSave;
using UnityEngine.UI;
using System;
using TMPro;

public class CharacterInfoToggles : MonoBehaviour
{
    [Header("토글 프리팹 설정")]
    [SerializeField] private GameObject characterTogglePrefab;
    [SerializeField] private Transform toggleParent;
    [SerializeField] private ToggleGroup toggleGroup;

    [Header("선택된 캐릭터 정보 표시")]
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
        Unvoid();
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
    }

    void Unvoid()
    {
        // 비어있는 함수
    }

    private void LoadCharacterData()
    {
        // ⭐ 참조가 아닌 새로운 리스트로 복사하여 중복 삭제 문제 해결
        characterDatas.Clear();
        characterDatas.AddRange(GameDataSaveLoadManager.Instance.GameData.characters);
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
                Destroy(toggleUI.toggle.gameObject);
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
        CharacterData capturedCharacter = character;

        toggleUI.toggle.onValueChanged.RemoveAllListeners();
        toggleUI.toggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                OnToggleValueChanged(toggleUI.toggle, capturedCharacter, capturedIndex);
            }
        });
    }

    private void UpdateToggleUI(CharacterToggleUI toggleUI, CharacterData character)
    {
        if (toggleUI.characterNameText != null)
            toggleUI.characterNameText.text = $"Lv. {character.level} {character.characterName}";

        if (toggleUI.characterClassText != null)
            toggleUI.characterClassText.text = $"{character.jobData.jobName}";

        // ⭐ 직업 이미지 설정 추가
        if (toggleUI.characterImage != null && character.jobData != null)
        {
            if (character.jobData.jobIcon != null)
            {
                toggleUI.characterImage.sprite = character.jobData.jobIcon;
            }
            else
            {
                Debug.LogWarning($"캐릭터 {character.characterName}의 직업 이미지가 없습니다!");
            }
        }

    }

    private void OnToggleValueChanged(Toggle toggle, CharacterData character, int index)
    {
        selectedCharacter = character;
        selectedCharacterIndex = index;

        UpdateSelectedCharacterInfo();
        GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(index);
        OnCharacterSelected?.Invoke(character, index);
    }

    private void UpdateSelectedCharacterInfo()
    {
        if (selectedCharacter == null) return;

        if (selectedCharacterPanel != null)
            selectedCharacterPanel.SetActive(true);

        if (selectedCharacterName != null)
            selectedCharacterName.text = selectedCharacter.characterName;
        if (selectedCharacterClass != null)
            selectedCharacterClass.text = $"레벨: {selectedCharacter.level}";
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
        GameDataSaveLoadManager.Instance.GameData.characters.Add(newCharacter);
        GameDataSaveLoadManager.Instance.SaveGame();
        RefreshCharacterData();
    }

    // ⭐ 삭제 함수 수정 - 중복 삭제 문제 해결
    public void DeleteSpecificCharacter(CharacterData targetCharacter, int index)
    {
        if (targetCharacter == null)
        {
            Debug.LogWarning("삭제할 캐릭터가 null입니다!");
            return;
        }

        // 최신 데이터로 다시 로드
        LoadCharacterData();

        // 유효성 검사
        if (index < 0 || index >= GameDataSaveLoadManager.Instance.GameData.characters.Count)
        {
            Debug.LogError($"삭제할 캐릭터 인덱스가 유효하지 않습니다! 인덱스: {index}");
            return;
        }

        // ⭐ GameData.characters에서만 삭제 (characterDatas는 다음 RefreshCharacterData에서 자동 갱신됨)
        GameDataSaveLoadManager.Instance.GameData.characters.RemoveAt(index);
        GameDataSaveLoadManager.Instance.SaveGame();

        Debug.Log($"캐릭터 삭제 완료. 남은 캐릭터 수: {GameDataSaveLoadManager.Instance.GameData.characters.Count}");

        // 선택된 캐릭터 관련 처리
        if (selectedCharacterIndex == index)
        {
            selectedCharacter = null;
            selectedCharacterIndex = -1;
            GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(-1);
        }
        else if (selectedCharacterIndex > index)
        {
            // 선택된 캐릭터가 삭제된 캐릭터보다 뒤에 있다면 인덱스 조정
            selectedCharacterIndex--;
            GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(selectedCharacterIndex);
        }

        // UI 갱신
        RefreshCharacterData();

        // 남은 캐릭터가 있다면 첫 번째 캐릭터 자동 선택
        if (GameDataSaveLoadManager.Instance.GameData.characters.Count > 0)
        {
            SelectCharacterByIndex(0);
        }
        else
        {
            if (selectedCharacterPanel != null)
                selectedCharacterPanel.SetActive(false);
        }
    }

    public void ForceRefresh()
    {
        RefreshCharacterData();
    }
}