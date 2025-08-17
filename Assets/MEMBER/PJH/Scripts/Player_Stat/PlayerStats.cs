using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
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

    public int GetRandomDamage()
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

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
    public static event Action OnPlayerDied;
    private PlayerStateMachine stateMachine;
    private bool isDead = false;

    public string characterName;
    public string characterJob;

    public event Action OnManaChanged;
    public event Action OnStaminaChanged;
    public event Action OnEXPChanged;
    public event Action OnLevelChanged;
    public Action OnHealthChanged;

    [Header("현재 상태")]
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _currentMana;
    [SerializeField] private int _currentStamina;
    [SerializeField] private int _level;
    [SerializeField] private int _currentEXP;

    // 이전 값들을 저장해서 변화를 감지
    private int _previousHealth;
    private int _previousMana;
    private int _previousStamina;
    private int _previousLevel;
    private int _previousEXP;

    private Coroutine _regenerateCoroutine;

    public int currentHealth
    {
        get => _currentHealth;
        set
        {
            if (_currentHealth != value)
            {
                _currentHealth = value;
                Debug.Log($"PlayerStats: currentHealth setter 호출 {_currentHealth} -> {value}");
                TriggerHealthChanged();
            }
        }
    }

    public int currentMana
    {
        get => _currentMana;
        set
        {
            if (_currentMana != value)
            {
                _currentMana = value;
                TriggerManaChanged();
            }
        }
    }

    public int currentStamina
    {
        get => _currentStamina;
        set
        {
            if (_currentStamina != value)
            {
                _currentStamina = value;
                TriggerStaminaChanged();
            }
        }
    }

    public int level
    {
        get => _level;
        set
        {
            if (_level != value)
            {
                _level = value;
                TriggerLevelChanged();
            }
        }
    }

    public int currentEXP
    {
        get => _currentEXP;
        set
        {
            if (_currentEXP != value)
            {
                _currentEXP = value;
                TriggerEXPChanged();
            }
        }
    }

    [Header("기본 능력치")]
    public Stat maxHealth;
    public Stat maxMana;
    public Stat manaRegen;
    public Stat maxStamina;
    public Stat staminaRegen;
    public Stat maxEXP;
    public Stat defense;
    public Stat magicDefense;
    public Stat Str;
    public Stat Dex;
    public Stat Int;

    [Header("이동 관련 능력치")]
    public FloatStat moveSpeed;
    public FloatStat sprintSpeed;

    [Header("전투 관련 능력치")]
    public Stat attackDamage;
    public Stat magicDamage;
    public FloatStat attackSpeed;

    [Header("크리티컬 관련 능력치")]
    public FloatStat criRate;
    public FloatStat criDamage;
    public FloatStat criResist;

    public PlayerDamageRange attackDamageRange;
    public float damageRange = 0.2f;

    public bool IsBerserk { get; private set; } = false; 

    public Vector3 currentPos;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        stateMachine = GetComponent<PlayerStateMachine>();
        ApplyBaseStats();
        OnHealthChanged += () => HealthCheck();

        // 초기 이전 값들 설정
        UpdatePreviousValues();
    }

    private void Start()
    {
        if (_regenerateCoroutine != null)
        {
            StopCoroutine(_regenerateCoroutine);
        }
        _regenerateCoroutine = StartCoroutine(RegenerateResources());
    }

    // Unity Editor에서 Inspector 값이 변경될 때 호출됨
    void OnValidate()
    {
        // 에디터에서만 실행되도록 제한
        if (!Application.isPlaying) return;

        // 각 값이 변경되었는지 확인하고 이벤트 호출
        if (_currentHealth != _previousHealth)
        {
            Debug.Log($"PlayerStats: OnValidate - Health 변경 감지 {_previousHealth} -> {_currentHealth}");
            TriggerHealthChanged();
            _previousHealth = _currentHealth;
        }

        if (_currentMana != _previousMana)
        {
            TriggerManaChanged();
            _previousMana = _currentMana;
        }

        if (_currentStamina != _previousStamina)
        {
            TriggerStaminaChanged();
            _previousStamina = _currentStamina;
        }

        if (_level != _previousLevel)
        {
            TriggerLevelChanged();
            _previousLevel = _level;
        }

        if (_currentEXP != _previousEXP)
        {
            TriggerEXPChanged();
            _previousEXP = _currentEXP;
        }
    }

    // 이전 값들을 현재 값으로 업데이트
    private void UpdatePreviousValues()
    {
        _previousHealth = _currentHealth;
        _previousMana = _currentMana;
        _previousStamina = _currentStamina;
        _previousLevel = _level;
        _previousEXP = _currentEXP;
    }

    // 각 이벤트 호출 메서드들
    private void TriggerHealthChanged()
    {
        Debug.Log($"PlayerStats: TriggerHealthChanged 호출됨 (현재 HP: {_currentHealth})");
        OnHealthChanged?.Invoke();
        if (OnHealthChanged != null)
        {
            Debug.Log($"PlayerStats: OnHealthChanged 이벤트 호출됨 (구독자 수: {OnHealthChanged.GetInvocationList().Length})");
        }
        else
        {
            Debug.LogWarning("PlayerStats: OnHealthChanged 이벤트에 구독자가 없습니다!");
        }
        UpdatePreviousValues();
    }

    private void TriggerManaChanged()
    {
        OnManaChanged?.Invoke();
        UpdatePreviousValues();
    }

    private void TriggerStaminaChanged()
    {
        OnStaminaChanged?.Invoke();
        UpdatePreviousValues();
    }

    private void TriggerLevelChanged()
    {
        OnLevelChanged?.Invoke();
        UpdatePreviousValues();
    }

    private void TriggerEXPChanged()
    {
        OnEXPChanged?.Invoke();
        UpdatePreviousValues();
    }

    private IEnumerator RegenerateResources()
    {
        while (!isDead)
        {
            if (currentMana < maxMana.GetValue())
            {
                currentMana += manaRegen.GetValue();
                currentMana = Mathf.Min(currentMana, maxMana.GetValue());
            }

            bool canRegenStamina = (stateMachine.currentState is PlayerIdleState || stateMachine.currentState is PlayerMoveState)
                                   && !stateMachine.IsSprinting;

            if (canRegenStamina && currentStamina < maxStamina.GetValue())
            {
                currentStamina += staminaRegen.GetValue();
                currentStamina = Mathf.Min(currentStamina, maxStamina.GetValue());
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public int GetAttackDamage()
    {
        var currentDamageRange = new PlayerDamageRange(attackDamage.GetValue(), damageRange);
        return currentDamageRange.CalculateDamage(criRate.GetValue(), criDamage.GetValue());
    }

    public int GetSkillDamage(SkillData skill)
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

        UpdateFinalStats();
    }

    public void TakePhysicalDamage(int damage)
    {
        if (isDead) return;
        int finalDamage = CheckTargetArmor(this, damage);
        Debug.Log($"PlayerStats: TakePhysicalDamage - 원본 데미지: {damage}, 최종 데미지: {finalDamage}, 방어력: {defense.GetValue()}");

        DecreaseHealth(finalDamage);
        Debug.Log($"플레이어가 {finalDamage}의 물리 피해를 입었습니다. 현재 체력: {currentHealth}");
    }

    public void TakeMagicalDamage(int damage)
    {
        if (isDead) return;
        int finalDamage = CheckTargetMagicArmor(this, damage);

        DecreaseHealth(finalDamage);
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
        if (IsBerserk && currentHealth - finalDamage <= 0)
        {
            currentHealth = 1;
        }
        else
        {
            currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        }
        return currentHealth;
    }

    protected virtual int CheckTargetArmor(PlayerStats target, int _damage)
    {
        int reducedDamage = _damage - target.defense.GetValue();
        return Mathf.Max(reducedDamage, 1);
    }

    protected virtual int CheckTargetMagicArmor(PlayerStats target, int _damage)
    {
        int reducedDamage = _damage - target.magicDefense.GetValue();
        return Mathf.Max(reducedDamage, 1);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        if (_regenerateCoroutine != null)
        {
            StopCoroutine(_regenerateCoroutine);
            _regenerateCoroutine = null;
        }
        stateMachine?.Die();
        OnPlayerDied?.Invoke();
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
        float moveSpeedBonus = 0;
        float sprintSpeedBonus = 0;

        List<Stat> statsToModify = new List<Stat>();
        foreach (var buff in skill.buffs)
        {
            Stat targetStat = GetStat(buff.statToBuff);
            if (targetStat != null)
            {
                statsToModify.Add(targetStat);
                if (buff.statToBuff == StatType.maxStamina)
                {
                    currentStamina += buff.amount;
                }
            }
        }

        for (int i = 0; i < statsToModify.Count; i++)
        {
            statsToModify[i].AddModifier(skill.buffs[i].amount);
        }

        if (skill.Hasting)
        {
            stateMachine.Animator.speed = skill.hasteAmount;  // 애니메이터 속도 증가

            moveSpeedBonus = moveSpeed.GetValue() * (skill.hasteAmount - 1f);
            sprintSpeedBonus = sprintSpeed.GetValue() * (skill.hasteAmount - 1f);

            moveSpeed.AddModifier(moveSpeedBonus);
            sprintSpeed.AddModifier(sprintSpeedBonus);
        }
        if (skill.Berserk)
        {
            IsBerserk = true;
        }

        UpdateFinalStats();
        // --------------------------------------------------------- 버프적용
        yield return new WaitForSeconds(skill.buffDuration);
        // ---------------------------------------------------------- 버프지속

        for (int i = 0; i < statsToModify.Count; i++)
        {
            statsToModify[i].RemoveModifier(skill.buffs[i].amount);
        }
        currentStamina = Mathf.Min(currentStamina, maxStamina.GetValue());
        if (skill.Hasting)
        {
            stateMachine.Animator.speed = 1f;

            moveSpeed.RemoveModifier(moveSpeedBonus);
            sprintSpeed.RemoveModifier(sprintSpeedBonus);
        }
        if (skill.Berserk)
        {
            IsBerserk = false;
        }

        UpdateFinalStats();
        //---------------------------------------------------------- 버프해제
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
            case StatType.maxStamina: return maxStamina;
            default: return null;
        }
    }

    public void LoadData(PlayerSaveData data)
    {
        if (data == null) return;

        this.characterName = data.characterName;
        this.characterJob = data.characterJob;
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
        transform.rotation = Quaternion.Euler(data.saveRot);
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
        maxHealth.AddModifier(baseStats.maxHealthPerLevel);
        maxMana.AddModifier(baseStats.maxManaPerLevel);
        maxStamina.AddModifier(baseStats.maxStaminaPerLevel);
        defense.AddModifier(baseStats.defensePerLevel);
        magicDefense.AddModifier(baseStats.magicDefensePerLevel);
        Str.AddModifier(baseStats.strPerLevel);
        Dex.AddModifier(baseStats.dexPerLevel);
        Int.AddModifier(baseStats.intPerLevel);

        currentHealth = maxHealth.GetValue(); // 체력과 마나를 최대치로 회복
        currentMana = maxMana.GetValue();

        maxEXP.SetDefaultValue(baseStats.maxEXP + 100 * (level - 1));

        UpdateFinalStats();  // 최종 스탯 업뎃
        Debug.Log("스탯 재계산 완료");
    }

    public void UpdateFinalStats()
    {
        int strBonusDamage = Str.GetValue() * 1;
        int dexBonusDamage = Dex.GetValue() * 1;
        attackDamage.SetDefaultValue(baseStats.attackDamage + strBonusDamage + dexBonusDamage);
        Debug.Log($"공격력 업데이트: {attackDamage.GetValue()} (기본: {baseStats.attackDamage}, 힘 보너스: {strBonusDamage}, 민첩 보너스: {dexBonusDamage})");
    }

    // 테스트용 메서드 (Inspector에서 테스트용)
    [ContextMenu("Test Take Damage (무방어력)")]
    public void TestTakeDamageIgnoreDefense()
    {
        currentHealth -= 10; // 방어력 무시하고 직접 10 감소
        Debug.Log($"테스트: HP 10 직접 감소, 현재 HP: {currentHealth}");
    }

    public void Respawn()
    {
        isDead = false;
        stateMachine.Animator.SetTrigger("Respawn");
        currentHealth = maxHealth.GetValue();
        currentMana = maxMana.GetValue();
        currentStamina = maxStamina.GetValue();
        stateMachine.ChangeState(stateMachine.IdleState);
        if (_regenerateCoroutine != null)
        {
            StopCoroutine(_regenerateCoroutine);
        }
        _regenerateCoroutine = StartCoroutine(RegenerateResources());
    }
}