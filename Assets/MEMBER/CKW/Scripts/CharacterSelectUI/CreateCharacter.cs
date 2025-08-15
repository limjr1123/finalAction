using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateCharacter : MonoBehaviour
{
    [SerializeField] GameObject nicknameWindow;
    [SerializeField] Button nicknameButton;
    [SerializeField] Button closeButton;
    [SerializeField] GameObject characterCreateWindow;
    [SerializeField] GameObject characterSelectWindow;

    [Header("Nickname Input")]
    [SerializeField] TMP_InputField nicknameInputField;

    private string lastValidText = "";
    private bool isProcessing = false;

    void Start()
    {
        if (nicknameButton != null)
            nicknameButton.onClick.AddListener(CreateCharacters);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseNickname);

        if (nicknameInputField != null)
        {
            // 모바일에서는 기본 제한을 사용하지 않음
            nicknameInputField.characterLimit = 0;
            nicknameInputField.onValidateInput = null;

            // 텍스트 변경 이벤트만 사용
            nicknameInputField.onValueChanged.AddListener(OnTextChanged);

            lastValidText = "";
        }
    }

    private void OnTextChanged(string newText)
    {
        if (isProcessing) return;

        isProcessing = true;

        // 허용된 문자만 남기기
        string cleanText = Regex.Replace(newText, @"[^0-9a-zA-Z가-힣]", "");

        // 6글자 초과 시 이전 유효한 텍스트로 되돌리기
        string finalText;
        if (cleanText.Length > 6)
        {
            finalText = lastValidText; // 7글자가 되려고 하면 이전 상태로 되돌림
        }
        else
        {
            finalText = cleanText;
            lastValidText = finalText; // 유효한 텍스트 저장
        }

        // 텍스트가 바뀌었으면 업데이트
        if (finalText != newText)
        {
            nicknameInputField.text = finalText;
            nicknameInputField.caretPosition = finalText.Length;
        }

        isProcessing = false;
    }

    // 모바일에서는 Update로 지속적으로 모니터링
    void Update()
    {
        if (nicknameInputField != null && !isProcessing)
        {
            string currentText = nicknameInputField.text;

            // 6글자 초과하면 강제로 마지막 유효한 텍스트로 되돌리기
            if (currentText.Length > 6)
            {
                isProcessing = true;
                nicknameInputField.text = lastValidText;
                nicknameInputField.caretPosition = lastValidText.Length;
                isProcessing = false;
            }
        }
    }

    private void CreateCharacters()
    {
        string nickname = nicknameInputField.text.Trim();

        if (nickname.Length < 2)
        {
            Debug.LogWarning("닉네임은 최소 2글자 이상이어야 합니다!");
            return;
        }

        if (nickname.Length > 6)
        {
            Debug.LogWarning("닉네임은 최대 6글자까지만 가능합니다!");
            return;
        }

        JobData selectedJob = CharacterCreateManager.Instance?.GetSelectedJobData();
        if (selectedJob == null)
        {
            Debug.LogWarning("직업이 선택되지 않았습니다!");
            return;
        }

        GameDataSaveLoadManager.Instance.CreateCharacter(nickname, selectedJob);

        Debug.Log($"캐릭터 생성 완료: {nickname}, 직업: {selectedJob.jobName}");
        CloseNickname();
        characterCreateWindow.SetActive(false);
        characterSelectWindow.SetActive(true);
        RefreshCharacterUI();
        ResetCharacterCreation();
    }

    private void RefreshCharacterUI()
    {
        CharacterInfoToggles characterInfoToggles = FindAnyObjectByType<CharacterInfoToggles>(FindObjectsInactive.Include);

        if (characterInfoToggles != null)
        {
            characterInfoToggles.RefreshCharacterData();

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
        if (nicknameInputField != null)
        {
            nicknameInputField.text = "";
            lastValidText = "";
        }
    }
}