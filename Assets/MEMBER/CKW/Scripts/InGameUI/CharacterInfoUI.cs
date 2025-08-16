using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : BaseUI
{
    [SerializeField] Button closeButton;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterLevel;
    [SerializeField] TextMeshProUGUI characterJob;
    [SerializeField] TextMeshProUGUI characterMaxHp;
    [SerializeField] TextMeshProUGUI characterMaxMp;
    [SerializeField] TextMeshProUGUI characterStr;
    [SerializeField] TextMeshProUGUI characterDex;
    [SerializeField] TextMeshProUGUI characterInt;

    private PlayerStats playerStats;

    void Start()
    {
        closeButton.onClick.AddListener(CloseInfoUI);

        // PlayerStats 인스턴스 참조 가져오기
        playerStats = PlayerStats.Instance;

        // 초기 정보 업데이트
        UpdateCharacterInfo();

        // 이벤트 구독 (스탯이 변경될 때마다 UI 업데이트)
        if (playerStats != null)
        {
            playerStats.OnLevelChanged += UpdateCharacterInfo;
            playerStats.OnHealthChanged += UpdateCharacterInfo;
            playerStats.OnManaChanged += UpdateCharacterInfo;
        }
    }

    void OnEnable()
    {
        // UI가 활성화될 때마다 정보 업데이트
        UpdateCharacterInfo();
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        if (playerStats != null)
        {
            playerStats.OnLevelChanged -= UpdateCharacterInfo;
            playerStats.OnHealthChanged -= UpdateCharacterInfo;
            playerStats.OnManaChanged -= UpdateCharacterInfo;
        }
    }

    private void UpdateCharacterInfo()
    {
        if (playerStats == null)
        {
            playerStats = PlayerStats.Instance;
            if (playerStats == null) return;
        }

        // 캐릭터 기본 정보
        characterName.text = playerStats.characterName;
        characterLevel.text = $"Lv.{playerStats.level}";
        characterJob.text = playerStats.characterJob;

        // 체력/마나 최대치
        characterMaxHp.text = playerStats.maxHealth.GetValue().ToString();
        characterMaxMp.text = playerStats.maxMana.GetValue().ToString();

        // 기본 능력치
        characterStr.text = playerStats.Str.GetValue().ToString();
        characterDex.text = playerStats.Dex.GetValue().ToString();
        characterInt.text = playerStats.Int.GetValue().ToString();
    }

    private void CloseInfoUI()
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Select);
        CloseUI();
    }
}