using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatData", menuName = "Enemy Stat Setting/EnemyStatData", order = 1)]
public class EnemyStatData : ScriptableObject
{
    [Header("int Stat")]
    public int attackDamage;    //물리 공격력
    public int magicDamage;     //마법 공격력
    public int maxHealth;       //최대 체력
    public int defense;         //방어력
    public int magicDefense;    //마법 방어력
    public int mana;            //마나
    public int manaRegen;       //마나 회복 속도
    public int aggroRange;      //어그로 범위

    [Header("float Stat")]
    public float attackRange;     //공격 거리
    public float moveSpeed;       //이동 속도
    public float attackSpeed;     //공격 속도
    public float attackInterval;  //공격 주기
    public float criticalChance;  //치명타 확률
    public float criticalDamage;  //치명타 피해량
}                               
