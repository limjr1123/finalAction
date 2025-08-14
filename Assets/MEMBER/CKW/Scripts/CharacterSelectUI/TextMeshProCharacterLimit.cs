using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProCharacterLimit : MonoBehaviour
{
    [Header("Character Limit Settings")]
    [SerializeField] private int maxCharacters = 6;
    [SerializeField] private bool allowOnlyKoreanEnglishNumbers = true;
    [SerializeField] private bool showWarningWhenLimitReached = true;

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;


    [Header("Events")]
    [SerializeField] private UnityEvent<string> onTextChanged;
    [SerializeField] private UnityEvent onLimitReached;

    private TextMeshProUGUI textMeshPro;
    private string previousValidText = "";
    private bool isUpdating = false; // 무한루프 방지 플래그

    void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        previousValidText = textMeshPro.text;
        normalColor = textMeshPro.color;
    }

    void Start()
    {
        // 초기 텍스트 검증
        ValidateText();
    }

    void Update()
    {
        // 업데이트 중이 아니고 텍스트가 변경되었을 때만 검증
        if (!isUpdating && textMeshPro.text != previousValidText)
        {
            ValidateText();
        }
    }

    private void ValidateText()
    {
        if (isUpdating) return; // 이미 업데이트 중이면 리턴

        isUpdating = true; // 업데이트 시작 플래그

        string currentText = textMeshPro.text;
        string validatedText = currentText;

        // 허용된 문자만 남기기 (옵션)
        if (allowOnlyKoreanEnglishNumbers)
        {
            validatedText = Regex.Replace(validatedText, @"[^0-9a-zA-Z가-힣]", "");
        }

        // 글자 수 제한
        if (validatedText.Length > maxCharacters)
        {
            validatedText = validatedText.Substring(0, maxCharacters);

            if (showWarningWhenLimitReached)
            {
                Debug.LogWarning($"텍스트가 {maxCharacters}글자로 제한되었습니다: {validatedText}");
            }

            // 제한 도달 이벤트 호출
            onLimitReached?.Invoke();
        }

        // 텍스트가 변경되었다면 업데이트
        if (currentText != validatedText)
        {
            textMeshPro.text = validatedText;
            textMeshPro.ForceMeshUpdate();

            // 텍스트 변경 이벤트 호출
            onTextChanged?.Invoke(validatedText);
        }

        // 색상 변경 (옵션)


        previousValidText = validatedText;
        isUpdating = false; // 업데이트 완료 플래그
    }

    // 공개 메서드들
    public void SetMaxCharacters(int max)
    {
        maxCharacters = max;
        ValidateText();
    }

    public void SetText(string newText)
    {
        if (isUpdating) return; // 업데이트 중이면 무시

        textMeshPro.text = newText;
        ValidateText();
    }

    public string GetText()
    {
        return textMeshPro.text;
    }

    public int GetCurrentLength()
    {
        return textMeshPro.text.Length;
    }

    public int GetRemainingCharacters()
    {
        return maxCharacters - textMeshPro.text.Length;
    }

    public bool IsAtLimit()
    {
        return textMeshPro.text.Length >= maxCharacters;
    }

    public void ClearText()
    {
        SetText("");
    }

    public void AppendText(string textToAdd)
    {
        if (isUpdating) return;

        string newText = textMeshPro.text + textToAdd;
        SetText(newText);
    }

    // 외부에서 직접 텍스트를 변경했을 때 호출
    public void ForceValidate()
    {
        ValidateText();
    }
}