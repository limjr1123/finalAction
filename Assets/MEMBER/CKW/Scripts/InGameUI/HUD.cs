using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameSave;

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

    // 캐릭터 정보 표시용 UI
    [Header("Character Info Display")]
    [SerializeField] TextMeshProUGUI characterNameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image manaBar;
    [SerializeField] TextMeshProUGUI manaText;
    [SerializeField] Image staminaBar;
    [SerializeField] TextMeshProUGUI staminaText;
    [SerializeField] Image expBar;
    [SerializeField] TextMeshProUGUI expText;

    // 현재 플레이어 참조
    private PlayerStats playerStats;
    private bool isInitialized = false;

    void Start()
    {
        // 버튼 이벤트 등록
        RegisterButtonEvents();

        // PlayerStats가 아직 연결되지 않았다면 자동으로 찾아서 연결 시도
        if (playerStats == null)
        {
            StartCoroutine(AutoFindPlayerStats());
        }
    }

    private System.Collections.IEnumerator AutoFindPlayerStats()
    {
        // 몇 프레임 기다린 후 PlayerStats 찾기
        yield return new WaitForSeconds(0.1f);

        PlayerStats foundStats = FindAnyObjectByType<PlayerStats>();
        if (foundStats != null)
        {
            Debug.Log("HUD: PlayerStats 자동 검색으로 발견됨");
            InitializeWithPlayer(foundStats);
        }
    }

    private void RegisterButtonEvents()
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

    void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        UnsubscribeFromEvents();
    }

    void OnEnable()
    {
        SubscribeToEvents();
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    // 플레이어 스탯으로 HUD 초기화
    public void InitializeWithPlayer(PlayerStats stats)
    {
        Debug.Log($"HUD: InitializeWithPlayer 호출됨. 전달받은 PlayerStats: {stats != null}");

        // 이미 초기화되어 있고 같은 PlayerStats라면 재초기화하지 않음
        if (isInitialized && playerStats == stats)
        {
            Debug.Log("HUD: 이미 같은 PlayerStats로 초기화되어 있음");
            return;
        }

        // 기존 이벤트 구독 해제
        UnsubscribeFromEvents();

        playerStats = stats;

        if (playerStats == null)
        {
            Debug.LogWarning("HUD: PlayerStats 컴포넌트가 전달되지 않았습니다.");
            isInitialized = false;
            return;
        }

        // 새로운 이벤트 구독
        SubscribeToEvents();

        // UI 업데이트
        UpdateAllUI();

        isInitialized = true;
        Debug.Log("HUD: 초기 UI 업데이트 완료");
    }

    // 이벤트 구독
    private void SubscribeToEvents()
    {
        if (playerStats != null)
        {
            Debug.Log("HUD: PlayerStats 이벤트 구독 시작");
            // PlayerStats의 각 이벤트 구독
            playerStats.OnHealthChanged += UpdateHealthUI;
            playerStats.OnManaChanged += UpdateManaUI;
            playerStats.OnStaminaChanged += UpdateStaminaUI;
            playerStats.OnEXPChanged += UpdateEXPUI;
            playerStats.OnLevelChanged += UpdateLevelUI;
            Debug.Log("HUD: PlayerStats 이벤트 구독 완료");
        }
    }

    // 이벤트 구독 해제
    private void UnsubscribeFromEvents()
    {
        if (playerStats != null)
        {
            Debug.Log("HUD: PlayerStats 이벤트 구독 해제");
            playerStats.OnHealthChanged -= UpdateHealthUI;
            playerStats.OnManaChanged -= UpdateManaUI;
            playerStats.OnStaminaChanged -= UpdateStaminaUI;
            playerStats.OnEXPChanged -= UpdateEXPUI;
            playerStats.OnLevelChanged -= UpdateLevelUI;
        }
    }

    // 전체 UI 업데이트 (초기화 시)
    private void UpdateAllUI()
    {
        if (playerStats == null) return;

        Debug.Log("HUD: 전체 UI 업데이트 시작");

        // 기본 정보 업데이트
        if (characterNameText != null)
            characterNameText.text = playerStats.characterName;

        UpdateLevelUI();
        UpdateHealthUI();
        UpdateManaUI();
        UpdateStaminaUI();
        UpdateEXPUI();

        Debug.Log("HUD: 전체 UI 업데이트 완료");
    }

    // 개별 UI 업데이트 메서드들
    private void UpdateHealthUI()
    {
        if (playerStats == null) return;

        if (healthBar != null)
        {
            float fillAmount = playerStats.maxHealth.GetValue() > 0 ?
                (float)playerStats.currentHealth / playerStats.maxHealth.GetValue() : 0f;
            healthBar.fillAmount = fillAmount;
        }

        if (healthText != null)
        {
            healthText.text = $"{playerStats.currentHealth} / {playerStats.maxHealth.GetValue()}";
        }

        Debug.Log($"HUD: HP 업데이트 - {playerStats.currentHealth} / {playerStats.maxHealth.GetValue()}");
    }

    private void UpdateManaUI()
    {
        if (playerStats == null) return;

        if (manaBar != null)
        {
            float fillAmount = playerStats.maxMana.GetValue() > 0 ?
                (float)playerStats.currentMana / playerStats.maxMana.GetValue() : 0f;
            manaBar.fillAmount = fillAmount;
        }

        if (manaText != null)
        {
            manaText.text = $"{playerStats.currentMana} / {playerStats.maxMana.GetValue()}";
        }

        Debug.Log($"HUD: MP 업데이트 - {playerStats.currentMana} / {playerStats.maxMana.GetValue()}");
    }

    private void UpdateStaminaUI()
    {
        if (playerStats == null) return;

        if (staminaBar != null)
        {
            float fillAmount = playerStats.maxStamina.GetValue() > 0 ?
                (float)playerStats.currentStamina / playerStats.maxStamina.GetValue() : 0f;
            staminaBar.fillAmount = fillAmount;
        }

        if (staminaText != null)
        {
            staminaText.text = $"{playerStats.currentStamina} / {playerStats.maxStamina.GetValue()}";
        }
    }

    private void UpdateLevelUI()
    {
        if (playerStats == null) return;

        if (levelText != null)
        {
            levelText.text = $"Lv.{playerStats.level}";
        }
    }

    private void UpdateEXPUI()
    {
        if (playerStats == null) return;

        if (expBar != null)
        {
            float fillAmount = playerStats.maxEXP.GetValue() > 0 ?
                (float)playerStats.currentEXP / playerStats.maxEXP.GetValue() : 0f;
            expBar.fillAmount = fillAmount;
        }

        if (expText != null)
        {
            expText.text = $"{playerStats.currentEXP} / {playerStats.maxEXP.GetValue()}";
        }
    }

    // 테스트용 메서드 (디버그용)
    [ContextMenu("Test Health Decrease")]
    public void TestHealthDecrease()
    {
        if (playerStats != null)
        {
            playerStats.TakePhysicalDamage(10);
        }
    }

    // 버튼 이벤트들
    private void OnJumpButton()
    {
        // 점프 로직
    }

    private void OnEvasionButton()
    {
        // 회피 로직
    }

    private void OnCounterButton()
    {
        // 반격 로직
    }

    private void OnAttackButton()
    {
        // 공격 로직
    }

    private void OnSkillButton1()
    {
        // 스킬 1 로직
    }

    private void OnSkillButton2()
    {
        // 스킬 2 로직
    }

    private void OnConsumeButton1()
    {
        // 소모품 1 사용 로직
    }

    private void OnConsumeButton2()
    {
        // 소모품 2 사용 로직
    }
}