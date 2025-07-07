using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    [Header("기본 스탯")]
    public Stat moveSpeed;      //이동 속도

    public Stat attackDamage;   //물리 공격력
    public Stat magicDamage;    //마법 공격력
    public Stat attackSpeed;    //공격 속도
    public Stat attackRange;    //공격 사거리
    public Stat attackInterval; //공격 주기

    public Stat criticalChance; //치명타 확률
    public Stat criticalDamage; //치명타 피해량

    public Stat maxHealth;      //최대 체력
    public Stat currentHealth;  //현재 체력
    public Stat defense;        //방어력
    public Stat magicDefense;   //마법 방어력

    public Stat mana;           //마나
    public Stat manaRegen;      //마나 회복 속도


    private void Start()
    {
        GetMaxHealth();
    }


    private int GetMaxHealth()
    {
        return maxHealth.GetValue();
    }

}
