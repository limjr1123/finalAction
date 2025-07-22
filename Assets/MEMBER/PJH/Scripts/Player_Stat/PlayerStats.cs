using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatsData baseStats;

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
        ApplyBaseStats(); // 스탯 초기화
    }

    public void ApplyBaseStats()
    {
        if (baseStats == null)
        {
            Debug.LogError("Base Stats 데이터 필요");
            return;
        }

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


}
