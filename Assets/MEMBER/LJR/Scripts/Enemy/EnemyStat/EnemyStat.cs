using System;
using UnityEngine;

// 데미지 범위를 정의하는 구조체
[System.Serializable]
public struct DamageRange
{
    public int min;
    public int max;

    public DamageRange(int damage, float ratio)
    {
        min = Mathf.RoundToInt(damage * (1f - ratio));
        max = Mathf.RoundToInt(damage * (1f + ratio));
    }

    // 데미미지 범위에서 랜덤한 값을 반환(크리티컬 확률을 사용하지 않는 경우 사용 가능)
    public int GetRandomDamage()
    {
        return UnityEngine.Random.Range(min, max+1);
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

public class EnemyStat : MonoBehaviour
{
    [SerializeField] private EnemyStatData statData;

    [field: SerializeField] public int currentHealth { get; private set; }  //현재 체력

    [Header("기본 스탯")]
    public Stat attackDamage;   //물리 공격력
    public Stat magicDamage;    //마법 공격력
    public Stat maxHealth;      //최대 체력
    public Stat defense;        //방어력
    public Stat magicDefense;   //마법 방어력
    public Stat mana;           //마나
    public Stat manaRegen;      //마나 회복 속도
    public Stat aggroRange;     //어그로 범위

    public FloatStat attackRange;       //공격 사거리
    public FloatStat moveSpeed;         //이동 속도
    public FloatStat attackSpeed;       //공격 속도
    public FloatStat attackInterval;    //공격 주기
    public FloatStat criticalChance;    //치명타 확률
    public FloatStat criticalMultiplier; // 치명타 피해량

    protected Action OnHealthChanged;

    public DamageRange attackDamageRange;   // 공격 피해량 범위
    public float damageRange = 0.2f;        // 공격 피해량 범위 비율 (20%)

    private void Awake()
    {
        // ScriptableObject의 값을 Stat 객체에 할당
        InitializeStat();
        OnHealthChanged += () => HealthCheck();
        attackDamageRange = new DamageRange(attackDamage.GetValue(), damageRange); // 공격 피해 범위 초기화
    }

    protected void InitializeStat()
    {
        moveSpeed.SetDefaultValue(statData.moveSpeed);
        attackDamage.SetDefaultValue(statData.attackDamage);
        magicDamage.SetDefaultValue(statData.magicDamage);
        attackSpeed.SetDefaultValue(statData.attackSpeed);
        attackRange.SetDefaultValue(statData.attackRange);
        attackInterval.SetDefaultValue(statData.attackInterval);
        criticalChance.SetDefaultValue(statData.criticalChance);
        criticalMultiplier.SetDefaultValue(statData.criticalDamage);
        maxHealth.SetDefaultValue(statData.maxHealth);
        defense.SetDefaultValue(statData.defense);
        magicDefense.SetDefaultValue(statData.magicDefense);
        mana.SetDefaultValue(statData.mana);
        manaRegen.SetDefaultValue(statData.manaRegen);
        aggroRange.SetDefaultValue(statData.aggroRange);
        currentHealth = GetMaxHealth();
    }

    private int GetMaxHealth()
    {
        return maxHealth.GetValue();
    }

    // 물리 공격 피해
    public virtual void TakeAttackDamage(int _damage)
    {
        int finalDamage = CheckTargetArmor(this, _damage);

        DecreaseHealth(finalDamage);

        OnHealthChanged?.Invoke();
        Debug.Log($"{finalDamage} finalDamage");
    }

    private void HealthCheck()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void TakeMagicDamage(int _damage)
    {
        int finalDamage = CheckTargetMagicArmor(this, _damage);

        DecreaseHealth(finalDamage);

        OnHealthChanged?.Invoke();
        Debug.Log($"{finalDamage} finalDamage");
    }

    protected virtual int DecreaseHealth(int finalDamage)
    {
        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        return currentHealth;
    }

    protected virtual int CheckTargetArmor(EnemyStat target, int _damage)
    {
        // 방어력에 따른 피해 감소 로직
        int reducedDamage = _damage - target.defense.GetValue();
        return Mathf.Max(reducedDamage, 1); // 피해가 1 이하로 떨어지지 않도록 보정
    }

    protected virtual int CheckTargetMagicArmor(EnemyStat target, int _damage)
    {
        // 방어력에 따른 피해 감소 로직
        int reducedDamage = _damage - target.magicDefense.GetValue();
        return Mathf.Max(reducedDamage, 1); // 피해가 1 이하로 떨어지지 않도록 보정
    }

    protected virtual void Die()
    {
        // 적이 죽었을 때의 로직
        Debug.Log($"{gameObject.name} has died.");
        // 예: 오브젝트 파괴, 애니메이션 재생 등
        return;
    }
}