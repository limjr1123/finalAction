using System;
using UnityEngine;

[System.Serializable]
public struct PlayerDamageRange
{
    public int min;
    public int max;

    public PlayerDamageRange(int damage, float ratio)
    {
        min = Mathf.RoundToInt(damage * (1f - ratio));
        max = Mathf.RoundToInt(damage * (1f + ratio));
    }

    // 데미지 범위에서 랜덤한 값을 반환(크리티컬 확률을 사용하지 않는 경우 사용 가능)
    public int GetRandomDamage()
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    // 치명타 확률과 치명타 피해량 비율을 고려하여 최종 데미지 값을 반환
    public int CalculateDamage(float criticalChance, float criticalDamageRatio)
    {
        int baseDamage = GetRandomDamage();
        bool isCritical = UnityEngine.Random.Range(0f, 1f) < criticalChance;

        if (isCritical)
        {
            int damage = Mathf.RoundToInt(baseDamage * criticalDamageRatio);
            return damage;
        }
        else
        {
            return baseDamage;
        }
    }
}

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

    public PlayerDamageRange attackDamageRange;   // 공격 피해량 범위
    public float damageRange = 0.2f;        // 공격 피해량 범위 비율 (20%)

    protected Action OnHealthChanged;

    void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        ApplyBaseStats(); // 스탯 초기화
        OnHealthChanged += () => HealthCheck();
    }

    public int GetAttackDamage()
    {
        var currentDamageRange = new PlayerDamageRange(attackDamage.GetValue(), damageRange);
        return currentDamageRange.CalculateDamage(criRate.GetValue(), criDamage.GetValue());
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
        if (other.CompareTag("HitBox"))
        {
           Debug.Log("플레이어 피격");
        }
    }

    public void TakePhysicalDamage(int damage)
    {
        if (isDead) return; // 이미 사망한 경우 무시
        int finalDamage = CheckTargetArmor(this, damage);

        DecreaseHealth(finalDamage);

        OnHealthChanged?.Invoke();
        Debug.Log($"플레이어가 {finalDamage}의 물리 피해를 받았습니다. 현재 체력: {currentHealth}");
    }


    public void TakeMagicalDamage(int damage)
    {
        if (isDead) return; // 이미 사망한 경우 무시
        int finalDamage = CheckTargetMagicArmor(this, damage);

        DecreaseHealth(finalDamage);

        OnHealthChanged?.Invoke();
        Debug.Log($"플레이어가 {finalDamage}의 마법 피해를 받았습니다. 현재 체력: {currentHealth}");
    }

    private void HealthCheck()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual int DecreaseHealth(int finalDamage)
    {
        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        return currentHealth;
    }

    protected virtual int CheckTargetArmor(PlayerStats target, int _damage)
    {
        // 방어력에 따른 피해 감소 로직
        int reducedDamage = _damage - target.defense.GetValue();
        return Mathf.Max(reducedDamage, 1); // 피해가 1 이하로 떨어지지 않도록 보정
    }

    protected virtual int CheckTargetMagicArmor(PlayerStats target, int _damage)
    {
        // 방어력에 따른 피해 감소 로직
        int reducedDamage = _damage - target.magicDefense.GetValue();
        return Mathf.Max(reducedDamage, 1); // 피해가 1 이하로 떨어지지 않도록 보정
    }

    private void Die()
    {
        if (isDead) return; // 중복 실행 방지

        isDead = true;
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
