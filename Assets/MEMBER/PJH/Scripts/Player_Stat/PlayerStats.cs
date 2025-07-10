using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯")]
    public Stat maxHealth; // 최대 체력
    public Stat currentHealth; // 현재 체력

    public Stat maxMana; // 최대 마나
    public Stat currentMana; // 현재 마나
    public Stat manaRegen; // 마나 회복 속도

    public Stat maxStamina; // 최대 스태미나
    public Stat currentStamina; // 현재 스태미나
    public Stat staminaRegen; // 스태미나 회복 속도

    public Stat defense; // 방어력
    public Stat magicDefense; // 마법 방어력

    public Stat Str; // 힘
    public Stat Dex; // 민첩
    public Stat Int; // 지능

    [Header("이동 관련 스탯")]
    public Stat moveSpeed; // 기본 이동속도
    public Stat sprintSpeed; // 달리기 속도

    [Header("공격 관련 스탯")]
    public Stat attackDamage; // 물리 공격력
    public Stat magicDamage; // 마법 공격력
    public Stat attackSpeed; // 공격 속도

    [Header("치명타 관련 스탯")]
    public Stat criRate; // 치명타 확률
    public Stat criDamage; // 치명타 피해량
    public Stat criResist; // 치명타 저항

    [Header("레벨 관련 스탯")]
    public Stat level; // 플레이어 레벨
    public Stat maxEXP; // 최대 경험치
    public Stat currentEXP; // 현재 경험치
}
