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
    [SerializeField] GameObject panel;

    [Header("Nickname Input")]
    [SerializeField] TMP_InputField nicknameInputField;

    private string lastValidText = "";
    private bool isProcessing = false;
    private Coroutine validationCoroutine;

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

            // 텍스트 변경 이벤트 사용
            nicknameInputField.onValueChanged.AddListener(OnTextChanged);
            // 입력 종료 이벤트도 추가
            nicknameInputField.onEndEdit.AddListener(OnEndEdit);

            lastValidText = "";
        }
    }

    private void OnTextChanged(string newText)
    {
        if (isProcessing) return;

        // Coroutine 대신 즉시 검증
        ValidateAndCleanText(newText);
    }

    private void OnEndEdit(string finalText)
    {
        // 입력이 완료되었을 때 최종 검증
        if (validationCoroutine != null)
        {
            StopCoroutine(validationCoroutine);
        }
        ValidateAndCleanText(finalText);
    }

    private void ValidateAndCleanText(string newText)
    {
        if (isProcessing) return;

        isProcessing = true;

        // 더 관대한 한글 범위 사용 (조합 중인 한글도 포함)
        // 0x1100-0x11FF: 한글 자모 (초성, 중성, 종성)
        // 0x3130-0x318F: 한글 호환 자모
        // 0xAC00-0xD7AF: 한글 완성형
        string cleanText = Regex.Replace(newText, @"[^0-9a-zA-Z\u1100-\u11FF\u3130-\u318F\uAC00-\uD7AF]", "");

        // 글자 수 계산 (한글은 조합 중일 수도 있으므로 더 유연하게)
        string finalText;
        if (GetVisualCharacterCount(cleanText) > 6)
        {
            // 6글자를 초과하면 마지막 유효한 텍스트 사용
            finalText = lastValidText;
        }
        else
        {
            finalText = cleanText;
            // 조합이 완료된 것 같으면 저장
            if (!IsKoreanComposing(finalText))
            {
                lastValidText = finalText;
            }
        }

        // 텍스트가 바뀌었으면 업데이트
        if (finalText != newText)
        {
            nicknameInputField.text = finalText;
            nicknameInputField.caretPosition = finalText.Length;
        }

        isProcessing = false;
    }

    // 시각적으로 보이는 글자 수를 계산 (조합 중인 한글도 고려)
    private int GetVisualCharacterCount(string text)
    {
        int count = 0;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            // 한글 완성형이거나 영문/숫자면 1글자로 카운트
            if ((c >= 0xAC00 && c <= 0xD7AF) || // 한글 완성형
                (c >= '0' && c <= '9') ||        // 숫자
                (c >= 'a' && c <= 'z') ||        // 영문 소문자
                (c >= 'A' && c <= 'Z'))          // 영문 대문자
            {
                count++;
            }
            // 한글 자모는 조합 중일 수 있으므로 더 복잡한 로직이 필요하지만
            // 일단 간단하게 처리
            else if ((c >= 0x1100 && c <= 0x11FF) || // 한글 자모
                     (c >= 0x3130 && c <= 0x318F))   // 한글 호환 자모
            {
                count++;
            }
        }
        return count;
    }

    // 한글이 조합 중인지 확인 (단순화된 버전)
    private bool IsKoreanComposing(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;

        // 마지막 문자가 한글 자모이면 조합 중일 가능성이 높음
        char lastChar = text[text.Length - 1];
        return (lastChar >= 0x1100 && lastChar <= 0x11FF) || // 한글 자모
               (lastChar >= 0x3130 && lastChar <= 0x318F);   // 한글 호환 자모
    }

    // Update는 제거하거나 더 제한적으로 사용
    void Update()
    {
        // 모바일에서 한글 입력 시 Update는 방해가 될 수 있으므로 주석 처리
        /*
        if (nicknameInputField != null && !isProcessing)
        {
            string currentText = nicknameInputField.text;

            if (currentText.Length > 6)
            {
                isProcessing = true;
                nicknameInputField.text = lastValidText;
                nicknameInputField.caretPosition = lastValidText.Length;
                isProcessing = false;
            }
        }
        */
    }

    private void CreateCharacters()
    {
        string nickname = nicknameInputField.text.Trim();

        // 최종 검증에서는 완성된 한글만 허용
        string finalNickname = Regex.Replace(nickname, @"[^0-9a-zA-Z가-힣]", "");

        if (GetVisualCharacterCount(finalNickname) < 2)
        {
            Debug.LogWarning("닉네임은 최소 2글자 이상이어야 합니다!");
            return;
        }

        if (GetVisualCharacterCount(finalNickname) > 6)
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
        SoundManager.Instance.PlayUISFX(UISFXList.Select);
        GameDataSaveLoadManager.Instance.CreateCharacter(finalNickname, selectedJob);

        Debug.Log($"캐릭터 생성 완료: {finalNickname}, 직업: {selectedJob.jobName}");
        CloseNickname();
        characterCreateWindow.SetActive(false);
        characterSelectWindow.SetActive(true);
        panel.SetActive(false);
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
        SoundManager.Instance.PlayUISFX(UISFXList.Select);
        nicknameWindow.SetActive(false);
        panel.SetActive(false);
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