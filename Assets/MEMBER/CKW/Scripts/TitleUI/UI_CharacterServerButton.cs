// UI_CharacterServerButton.cs (맨 처음 상태 추정)
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameSave;
using System;
using System.Collections; // ForceUIUpdate 코루틴을 위해 필요

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

    [SerializeField] CharacterInfoToggles characterManager;

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

        // Debug.Log("UI_CharacterServerButton Start"); // 이 로그도 없었을 수 있습니다.
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
        // ⭐ 맨 처음에는 characterID 기반의 최신 인스턴스 검색 로직이 없었을 가능성이 높습니다.
        // 단순히 전달받은 character와 index를 사용했을 것입니다.
        selectedCharacterData = character;
        selectedCharacterIndex = index;

        // Debug.Log($"[UI_CharServerBtn] 캐릭터 선택됨: {selectedCharacterData.characterName} (인덱스: {selectedCharacterIndex})");
        // GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(selectedCharacterIndex); // 이것도 없었을 수 있습니다.
    }

    private void StartGame()
    {
        // selectedCharacterData = CharacterInfoToggles.GetCurrentSelectedCharacter(); // 이 로직은 나중에 추가되었을 수 있습니다.
        // 이미 OnCharacterSelectedHandler에서 할당되므로 이 줄이 없었을 수 있습니다.

        if (selectedCharacterData != null)
        {
            // Debug.Log($"게임 시작: {selectedCharacterData.characterName}"); // 이 로그도 없었을 수 있습니다.
            // GameDataSaveLoadManager.Instance.SetSelectedCharacterSlotIndex(selectedCharacterIndex); // 이 라인도 없었을 수 있습니다.
            SceneManager.LoadScene("Field");
        }
        else
        {
            Debug.LogWarning("선택된 캐릭터가 없습니다!");
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
        if (selectedCharacterData == null)
        {
            Debug.LogWarning("삭제할 캐릭터가 선택되지 않았습니다!");
            return;
        }

        // Debug.Log($"삭제 요청 - 캐릭터: {selectedCharacterData.characterName}, 저장된 인덱스: {selectedCharacterIndex}"); // 이 로그도 없었을 수 있습니다.

        if (characterManager != null)
        {
            var characterToDelete = selectedCharacterData; // 이 변수 할당도 없었을 수 있습니다.

            // DeleteSpecificCharacter에 인덱스만 전달했을 가능성도 있습니다.
            characterManager.DeleteSpecificCharacter(characterToDelete, selectedCharacterIndex);

            selectedCharacterData = null;
            selectedCharacterIndex = -1;

            // ForceUIUpdate 코루틴은 나중에 추가되었을 수 있습니다.
            StartCoroutine(ForceUIUpdate());
        }
    }

    private System.Collections.IEnumerator ForceUIUpdate()
    {
        yield return null;
        if (characterManager != null)
        {
            characterManager.ForceRefresh();
        }
        // var gameData = GameDataSaveLoadManager.Instance.GameData; // 이 로그도 없었을 수 있습니다.
        // Debug.Log($"UI 업데이트 후 확인 - 게임데이터 캐릭터 수: {gameData.characters.Count}"); // 이 로그도 없었을 수 있습니다.
    }
}