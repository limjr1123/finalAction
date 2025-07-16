using System;
using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    [SerializeField] private EnemyStatData statData;

    [Header("기본 스탯")]
    public Stat attackDamage;   //물리 공격력
    public Stat magicDamage;    //마법 공격력
    public Stat maxHealth;      //최대 체력
    public Stat defense;        //방어력
    public Stat magicDefense;   //마법 방어력
    public Stat mana;           //마나
    public Stat manaRegen;      //마나 회복 속도
    public Stat aggroRange;     //어그로 범위

    public FloatStat attackRange;    //공격 사거리
    public FloatStat moveSpeed;      //이동 속도
    public FloatStat attackSpeed;    //공격 속도
    public FloatStat attackInterval; //공격 주기
    public FloatStat criticalChance; //치명타 확률
    public FloatStat criticalDamage; //치명타 피해량

    protected Action OnHealthChanged;

    public int currentHealth { get; private set; }  //현재 체력

    private void Awake()
    {
        // ScriptableObject의 값을 Stat 객체에 할당
        InitializeStat();
        OnHealthChanged += () => HealthCheck();
    }

    private void Start()
    {
        currentHealth = GetMaxHealth();
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
        criticalDamage.SetDefaultValue(statData.criticalDamage);
        maxHealth.SetDefaultValue(statData.maxHealth);
        defense.SetDefaultValue(statData.defense);
        magicDefense.SetDefaultValue(statData.magicDefense);
        mana.SetDefaultValue(statData.mana);
        manaRegen.SetDefaultValue(statData.manaRegen);
        aggroRange.SetDefaultValue(statData.aggroRange);
    }

    private int GetMaxHealth()
    {
        return maxHealth.GetValue();
    }

    // 물리 공격 피해
    public virtual void TakeAttackDamage(int _damage)
    {
        _damage = CheckTargetArmor(this, _damage);

        DecreaseHealth(_damage);

        Debug.Log($"{_damage} _damage");
        OnHealthChanged?.Invoke();
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
        _damage = CheckTargetArmor(this, _damage);

        DecreaseHealth(_damage);

        Debug.Log($"{_damage} _damage");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual int DecreaseHealth(int damage)
    {
        return currentHealth -= damage;
    }

    protected virtual int CheckTargetArmor(EnemyStat target, int damage)
    {
        // 방어력에 따른 피해 감소 로직
        int reducedDamage = damage - target.defense.GetValue();
        return Mathf.Max(reducedDamage, 0); // 피해가 0 이하로 떨어지지 않도록 보장
    }

    protected virtual int CheckTargetMagicArmor(EnemyStat target, int damage)
    {
        // 방어력에 따른 피해 감소 로직
        int reducedDamage = damage - target.defense.GetValue();
        return Mathf.Max(reducedDamage, 0); // 피해가 0 이하로 떨어지지 않도록 보장
    }

    protected virtual void Die()
    {
        // 적이 죽었을 때의 로직
        Debug.Log($"{gameObject.name} has died.");
        // 예: 오브젝트 파괴, 애니메이션 재생 등
        Destroy(gameObject);
    }
}
