using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] 
    private PlayerStatsData baseStats;
    public static event Action OnPlayerDied; // 플레이어 사망 이벤트
    private PlayerStateMachine stateMachine;
    private bool isDead = false;

    public string characterName;
    public JobData jobData;

    [Header("현재상태")]
    public int currentHealth; // 현재 체력
    public int currentMana; // 현재 마나
    public int currentStamina; // 현재 스태미나
    public int level; // 플레이어 레벨
    public int currentEXP; // 현재 경험치

    [Header("기본 스탯")]
    public Stat maxHealth; // 최대 체력
    public Stat maxMana; // 최대 마나
    public Stat manaRegen; // 마나 회복 속도
    public Stat maxStamina; // 최대 스태미나
    public Stat staminaRegen; // 스태미나 회복 속도
    public Stat maxEXP; // 최대 경험치
    public Stat defense; // 방어력
    public Stat magicDefense; // 마법 방어력
    public Stat Str; // 힘
    public Stat Dex; // 민첩
    public Stat Int; // 지능

    [Header("이동 관련 스탯")]
    public FloatStat moveSpeed; // 기본 이동속도
    public FloatStat sprintSpeed; // 달리기 속도

    [Header("공격 관련 스탯")]
    public Stat attackDamage; // 물리 공격력
    public Stat magicDamage; // 마법 공격력
    public FloatStat attackSpeed; // 공격 속도

    [Header("치명타 관련 스탯")]
    public FloatStat criRate; // 치명타 확률
    public FloatStat criDamage; // 치명타 피해량
    public FloatStat criResist; // 치명타 저항

    void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        ApplyBaseStats(); // 스탯 초기화
    }

    public void ApplyBaseStats()
    {
        if (baseStats == null)
        {
            Debug.LogError("Base Stats 데이터 필요");
            return;
        }
        level = 1;
        maxEXP.SetDefaultValue(baseStats.maxEXP);
        currentEXP = 0;
        maxHealth.SetDefaultValue(baseStats.maxHealth);
        currentHealth = baseStats.maxHealth;
        maxMana.SetDefaultValue(baseStats.maxMana);
        currentMana = baseStats.maxMana;
        manaRegen.SetDefaultValue(baseStats.manaRegen);
        maxStamina.SetDefaultValue(baseStats.maxStamina);
        currentStamina = baseStats.maxStamina;
        staminaRegen.SetDefaultValue(baseStats.staminaRegen);
        defense.SetDefaultValue(baseStats.defense);
        magicDefense.SetDefaultValue(baseStats.magicDefense);
        moveSpeed.SetDefaultValue(baseStats.moveSpeed);
        sprintSpeed.SetDefaultValue(baseStats.sprintSpeed);
        attackDamage.SetDefaultValue(baseStats.attackDamage);
        magicDamage.SetDefaultValue(baseStats.magicDamage);
        attackSpeed.SetDefaultValue(baseStats.attackSpeed);
        criRate.SetDefaultValue(baseStats.criRate);
        criDamage.SetDefaultValue(baseStats.criDamage);
        criResist.SetDefaultValue(baseStats.criResist);
        Str.SetDefaultValue(baseStats.Str);
        Dex.SetDefaultValue(baseStats.Dex);
        Int.SetDefaultValue(baseStats.Int);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox")) // 태그는 실제 사용하는 것으로 변경
        {
            Debug.Log("플레이어 피격");

            int exDamage = 20;

            TakeDamage(exDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        // 방어력을 적용한 최종 데미지 계산 (최소 1의 데미지는 받도록 설정)
        currentHealth -= damage;
        Debug.Log($"{damage}의 데미지를 입었습니다. 현재 체력: {currentHealth}");

        if (currentHealth > 0)
        {
            // 아직 살아있다면 피격 상태로 전환
            stateMachine?.GetDamage();
        }
        else
        {
            // 체력이 0 이하면 사망 처리
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        if (isDead) return; // 중복 실행 방지

        isDead = true;
        currentHealth = 0;
        stateMachine?.Die();
        OnPlayerDied?.Invoke(); // 사망이벤트 호출
    }

    public void LoadData(PlayerSaveData data)
    {
        if (data == null)
        {
            return;
        }
        this.characterName = data.characterName;
        this.jobData = data.jobData;

        this.level = data.level;
        this.currentEXP = data.currentEXP;
        this.currentHealth = data.currentHealth;
        this.currentMana = data.currentMana;
        this.currentStamina = data.currentStamina;

        maxHealth.SetDefaultValue(data.maxHealth);
        maxMana.SetDefaultValue(data.maxMana);
        manaRegen.SetDefaultValue(data.manaRegen);
        maxStamina.SetDefaultValue(data.maxStamina);
        staminaRegen.SetDefaultValue(data.staminaRegen);
        maxEXP.SetDefaultValue(data.maxEXP);
        defense.SetDefaultValue(data.defense);
        magicDefense.SetDefaultValue(data.magicDefense);
        Str.SetDefaultValue(data.Str);
        Dex.SetDefaultValue(data.Dex);
        Int.SetDefaultValue(data.Int);
        moveSpeed.SetDefaultValue(data.moveSpeed);
        sprintSpeed.SetDefaultValue(data.sprintSpeed);
        attackDamage.SetDefaultValue(data.attackDamage);
        magicDamage.SetDefaultValue(data.magicDamage);
        attackSpeed.SetDefaultValue(data.attackSpeed);
        criRate.SetDefaultValue(data.criRate);
        criDamage.SetDefaultValue(data.criDamage);
        criResist.SetDefaultValue(data.criResist);

        Debug.Log($"데이터 적용 완료.");
    }
}
