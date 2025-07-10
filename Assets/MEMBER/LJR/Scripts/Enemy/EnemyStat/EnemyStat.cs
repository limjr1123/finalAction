using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    [SerializeField] private EnemyStatData statData;

    [Header("기본 스탯")]
    public Stat attackDamage;   //물리 공격력
    public Stat magicDamage;    //마법 공격력
    public Stat maxHealth;      //최대 체력
    public Stat currentHealth;  //현재 체력
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

    private void Awake()
    {
        // ScriptableObject의 값을 Stat 객체에 할당
        moveSpeed.SetDefaultValue(statData.moveSpeed);
        attackDamage.SetDefaultValue(statData.attackDamage);
        magicDamage.SetDefaultValue(statData.magicDamage);
        attackSpeed.SetDefaultValue(statData.attackSpeed);
        attackRange.SetDefaultValue(statData.attackRange);
        attackInterval.SetDefaultValue(statData.attackInterval);
        criticalChance.SetDefaultValue(statData.criticalChance);
        criticalDamage.SetDefaultValue(statData.criticalDamage);
        maxHealth.SetDefaultValue(statData.maxHealth);
        currentHealth.SetDefaultValue(statData.maxHealth);
        defense.SetDefaultValue(statData.defense);
        magicDefense.SetDefaultValue(statData.magicDefense);
        mana.SetDefaultValue(statData.mana);
        manaRegen.SetDefaultValue(statData.manaRegen);
        aggroRange.SetDefaultValue(statData.aggroRange);
    }

    private void Start()
    {
        currentHealth.SetDefaultValue(GetMaxHealth());
    }


    private int GetMaxHealth()
    {
        return maxHealth.GetValue();
    }

    
}
