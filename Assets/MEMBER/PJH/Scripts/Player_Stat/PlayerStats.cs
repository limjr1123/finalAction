using System;
using System.Collections;
using System.Collections.Generic;
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

    // 랜덤한 데미지를 계산해 반환 (퀄리티 확인용으로 변경하지 않는 한 그대로 사용)
    public int GetRandomDamage()
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    // 크리티컬 확률과 크리티컬 데미지 배율을 고려하여 최종 데미지를 반환
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
    public static PlayerStats Instance { get; private set; }

    [SerializeField]
    private PlayerStatsData baseStats;
    public static event Action OnPlayerDied; // 플레이어 사망 이벤트
    private PlayerStateMachine stateMachine;
    private bool isDead = false;

    public string characterName;
    public string characterJob;

    public event Action OnManaChanged;
    public event Action OnStaminaChanged;
    public event Action OnEXPChanged;
    public event Action OnLevelChanged;

    [Header("현재 상태")]
    [SerializeField] private int _currentHealth;
    public int currentHealth
    {
        get => _currentHealth;
        set
        {
            if (currentHealth != value)
            {
                _currentHealth = value;
                OnHealthChanged?.Invoke();
                Debug.Log($" hoho{OnHealthChanged}");
            }
        }
    }
    [SerializeField] private int _currentMana;
    public int currentMana
    {
        get => _currentMana;
        set
        {
            if (currentMana != value)
            {

                _currentMana = value;
                OnManaChanged?.Invoke();
            }
        }
    }
    [SerializeField] private int _currentStamina;
    public int currentStamina
    {
        get => _currentStamina;
        set
        {
            if (_currentStamina != value)
            {
                _currentStamina = value;
                OnStaminaChanged?.Invoke();
            }

        }
    }
    [SerializeField] private int _level;
    public int level
    {
        get => _level;
        set
        {
            if (_level != value)
            {

                _level = value;
                OnLevelChanged?.Invoke();
            }
        }
    }
    [SerializeField] private int _currentEXP;
    public int currentEXP
    {
        get => _currentEXP;
        set
        {
            if (_currentEXP != value)
            {

                _currentEXP = value;
                OnEXPChanged?.Invoke();
            }
        }
    }



    [Header("기본 능력치")]
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

    [Header("이동 관련 능력치")]
    public FloatStat moveSpeed; // 기본 이동속도
    public FloatStat sprintSpeed; // 달리기 속도

    [Header("전투 관련 능력치")]
    public Stat attackDamage; // 물리 공격력
    public Stat magicDamage; // 마법 공격력
    public FloatStat attackSpeed; // 공격 속도

    [Header("크리티컬 관련 능력치")]
    public FloatStat criRate; // 크리티컬 확률
    public FloatStat criDamage; // 크리티컬 데미지 배율
    public FloatStat criResist; // 크리티컬 저항

    public PlayerDamageRange attackDamageRange; // 물리 공격 데미지 범위
    public float damageRange = 0.2f; // 데미지 편차 범위 (20%)

    public Vector3 currentPos;

    public Action OnHealthChanged;



    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        stateMachine = GetComponent<PlayerStateMachine>();
        ApplyBaseStats(); // 기본 스탯 초기화
        OnHealthChanged += () => HealthCheck();
    }
    private void Start()
    {
        StartCoroutine(RegenerateResources());
    }

    private IEnumerator RegenerateResources()
    {
        while (!isDead)
        {
            // 마나 회복
            if (currentMana < maxMana.GetValue())
            {
                currentMana += manaRegen.GetValue();
                currentMana = Mathf.Min(currentMana, maxMana.GetValue());
            }

            // 스태미나 회복
            bool canRegenStamina = (stateMachine.currentState is PlayerIdleState || stateMachine.currentState is PlayerMoveState)
                                   && !stateMachine.IsSprinting;

            if (canRegenStamina && currentStamina < maxStamina.GetValue())
            {
                currentStamina += staminaRegen.GetValue();
                Debug.Log($"스태미나 회복: {currentStamina} / {maxStamina.GetValue()}");
                currentStamina = Mathf.Min(currentStamina, maxStamina.GetValue());
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public int GetAttackDamage()  // 공격 데미지 계산
    {
        var currentDamageRange = new PlayerDamageRange(attackDamage.GetValue(), damageRange);
        return currentDamageRange.CalculateDamage(criRate.GetValue(), criDamage.GetValue());
    }

    public int GetSkillDamage(SkillData skill) // 스킬 데미지 계산
    {
        int baseSkillDamage = Mathf.RoundToInt(attackDamage.GetValue() * skill.damageMultiplier);
        var currentDamageRange = new PlayerDamageRange(baseSkillDamage, damageRange);
        return currentDamageRange.CalculateDamage(criRate.GetValue(), criDamage.GetValue());
    }

    public void ApplyBaseStats()
    {
        if (baseStats == null)
        {
            Debug.LogError("Base Stats 데이터가 필요합니다.");
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
            Debug.Log("플레이어 피격!");
        }
    }

    public void TakePhysicalDamage(int damage)
    {
        if (isDead) return; // 이미 사망한 경우 무시
        int finalDamage = CheckTargetArmor(this, damage);

        DecreaseHealth(finalDamage);

        OnHealthChanged?.Invoke();
        Debug.Log($"플레이어가 {finalDamage}의 물리 피해를 입었습니다. 현재 체력: {currentHealth}");
    }

    public void TakeMagicalDamage(int damage)
    {
        if (isDead) return; // 이미 사망한 경우 무시
        int finalDamage = CheckTargetMagicArmor(this, damage);

        DecreaseHealth(finalDamage);

        OnHealthChanged?.Invoke();
        Debug.Log($"플레이어가 {finalDamage}의 마법 피해를 입었습니다. 현재 체력: {currentHealth}");
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
        // 대상의 방어력을 고려해 최종 데미지 계산
        int reducedDamage = _damage - target.defense.GetValue();
        return Mathf.Max(reducedDamage, 1); // 최소 1의 피해는 들어가도록 보정
    }

    protected virtual int CheckTargetMagicArmor(PlayerStats target, int _damage)
    {
        // 대상의 마법 방어력을 고려해 최종 데미지 계산
        int reducedDamage = _damage - target.magicDefense.GetValue();
        return Mathf.Max(reducedDamage, 1); // 최소 1의 피해는 들어가도록 보정
    }

    private void Die()
    {
        if (isDead) return; // 중복 사망 방지

        isDead = true;
        stateMachine?.Die();
        OnPlayerDied?.Invoke(); // 사망 이벤트 호출
    }

    public bool TryUseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            return true;
        }
        return false;
    }
    public bool TryUseStamina(int amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true;
        }
        return false;
    }
    public void ApplyBuff(SkillData skill)
    {
        StartCoroutine(BuffCoroutine(skill));
    }
    private IEnumerator BuffCoroutine(SkillData skill)
    {
        // 1. 적용할 모든 버프 스탯을 찾습니다.
        List<Stat> statsToModify = new List<Stat>();
        foreach (var buff in skill.buffs)
        {
            Stat targetStat = GetStat(buff.statToBuff);
            if (targetStat != null)
            {
                statsToModify.Add(targetStat);
            }
        }
        // 2. 찾아낸 모든 스탯에 버프를 적용합니다.
        for (int i = 0; i < statsToModify.Count; i++)
        {
            statsToModify[i].AddModifier(skill.buffs[i].amount);
            Debug.Log($"{skill.buffs[i].statToBuff} 스탯을 {skill.buffs[i].amount} 만큼 증가시킵니다.");
        }

        // 3. 지정된 시간만큼 기다립니다.
        yield return new WaitForSeconds(skill.buffDuration);

        // 4. 적용했던 모든 버프 효과를 제거합니다.
        for (int i = 0; i < statsToModify.Count; i++)
        {
            statsToModify[i].RemoveModifier(skill.buffs[i].amount);
            Debug.Log($"{skill.buffs[i].statToBuff} 스탯 버프가 종료되었습니다.");
        }
    }
    private Stat GetStat(StatType type)
    {
        switch (type)
        {
            case StatType.Str: return Str;
            case StatType.Dex: return Dex;
            case StatType.Int: return Int;
            case StatType.defense: return defense;
            case StatType.magicDefense: return magicDefense;
            default: return null;
        }
    }

    public void LoadData(PlayerSaveData data)
    {
        if (data == null)
        {
            return;
        }

        this.characterName = data.characterName;
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

        transform.position = data.savePos;
        Debug.Log($"캐릭터 데이터 로드 완료!");
    }

    public void AddExp(int amount)
    {
        currentEXP += amount;
        Debug.Log($"{amount}의 경험치를 획득했습니다. 현재 경험치: {currentEXP}/{maxEXP.GetValue()}");

        while (currentEXP >= maxEXP.GetValue())
        {
            currentEXP -= maxEXP.GetValue();
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        Debug.Log($"레벨 업! 현재 레벨: {level}");
    }

}
