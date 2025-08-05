using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Skill Buttons")]
    [SerializeField] Button jumpButton;
    [SerializeField] Button evasionButton;
    [SerializeField] Button counterButton;
    [SerializeField] Button attackButton;
    [SerializeField] Button skill_1;
    [SerializeField] Button skill_2;

    [Header("Consume Quick Slot")]
    [SerializeField] Button consume_1;
    [SerializeField] Button consume_2;

    // 캐릭터 정보 표시용 UI (필요시 추가)
    [Header("Character Info Display")]
    [SerializeField] Text characterNameText;
    [SerializeField] Text levelText;
    [SerializeField] Slider healthBar;
    [SerializeField] Slider manaBar;

    void Start()
    {
        if (jumpButton != null)
            jumpButton.onClick.AddListener(OnJumpButton);
        if (evasionButton != null)
            evasionButton.onClick.AddListener(OnEvasionButton);
        if (counterButton != null)
            counterButton.onClick.AddListener(OnCounterButton);
        if (attackButton != null)
            attackButton.onClick.AddListener(OnAttackButton);
        if (skill_1 != null)
            skill_1.onClick.AddListener(OnSkillButton1);
        if (skill_2 != null)
            skill_2.onClick.AddListener(OnSkillButton2);

        if (consume_1 != null)
            consume_1.onClick.AddListener(OnConsumeButton1);
        if (consume_2 != null)
            consume_2.onClick.AddListener(OnConsumeButton2);
    }

    // 캐릭터 데이터로 HUD 초기화 (UIManager에서 호출)
    public void InitializeWithCharacterData(int characterIndex)
    {
        Debug.Log($"HUD 캐릭터 데이터 초기화 - 인덱스: {characterIndex}");

        // GameDataSaveLoadManager에서 캐릭터 데이터 가져오기
        var characterData = GameDataSaveLoadManager.Instance.GetCharacterData(characterIndex);

        if (characterData != null)
        {
            // UI 업데이트 예시 (실제 PlayerSaveData 구조에 맞게 수정 필요)
            if (characterNameText != null)
                characterNameText.text = characterData.playerSaveData.characterName;
            if (levelText != null)
                levelText.text = $"Lv.{characterData.playerSaveData.level}";
            if (healthBar != null)
                healthBar.value = (float)characterData.playerSaveData.currentHealth / characterData.playerSaveData.maxHealth;
            if (manaBar != null)
                manaBar.value = (float)characterData.playerSaveData.currentMana / characterData.playerSaveData.maxMana;

            Debug.Log($"HUD 업데이트 완료 - 캐릭터: {characterData.playerSaveData.characterName}");
        }
        else
        {
            Debug.LogWarning($"캐릭터 데이터를 찾을 수 없습니다. 인덱스: {characterIndex}");
        }
    }

    // 스킬 & 공격 버튼 눌렀을 때
    private void OnJumpButton()
    {
    }

    private void OnEvasionButton()
    {
    }

    private void OnCounterButton()
    {
    }

    private void OnAttackButton()
    {
    }

    private void OnSkillButton1()
    {
    }

    private void OnSkillButton2()
    {
    }

    // 퀵슬롯(소모품) 눌렀을 때
    private void OnConsumeButton1()
    {
    }

    private void OnConsumeButton2()
    {
    }
}