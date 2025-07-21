using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "PlayerStatSetting/PlayerStatsData")]
public class PlayerStatsData : ScriptableObject
{
    [Header("int stat")]
    public int maxHealth; // 최대 체력
    public int maxMana; // 최대 마나
    public int manaRegen; // 마나 회복 속도
    public int maxStamina; // 최대 스태미나
    public int staminaRegen; // 스태미나 회복 속도
    public int defense; // 방어력
    public int magicDefense; // 마법 방어력
    public int Str; // 힘
    public int Dex; // 민첩
    public int Int; // 지능
    public int attackDamage; // 물리 공격력
    public int magicDamage; // 마법 공격력

    [Header("float stat")]
    public float moveSpeed; // 기본 이동속도
    public float sprintSpeed; // 달리기 속도
    public float attackSpeed; // 공격 속도
    public float criRate; // 치명타 확률
    public float criDamage; // 치명타 피해량
    public float criResist; // 치명타 저항

    [Header("레벨 관련 스탯")]
    public int level; // 플레이어 레벨
    public int maxEXP; // 최대 경험치
    public int currentEXP; // 현재 경험치
}
