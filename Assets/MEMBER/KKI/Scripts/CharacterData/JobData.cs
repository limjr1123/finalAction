using UnityEngine;

[CreateAssetMenu(fileName = "JobData", menuName = "Scriptable Objects/JobData")]
public class JobData : ScriptableObject
{
    public string jobName;
    public Sprite jobIcon;

    // 정수형 스탯 기본값
    public int baseHP = 150;
    public int baseMP = 100;
    public int baseManaRegen = 10;
    public int baseStamina = 100;
    public int baseStaminaRegen = 10;
    public int baseDefense = 20;
    public int baseMagicDefense = 20;
    public int baseStr = 15;
    public int baseDex = 10;
    public int baseInt = 10;
    public int baseAttack = 15;
    public int baseMagicDamage = 0;

    // 실수형 스탯 기본값
    public float baseMoveSpeed = 5f;
    public float baseSprintSpeed = 10f;
    public float baseAttackSpeed = 1f;
    public float baseCriRate = 0.2f;
    public float baseCriDamage = 1.5f;
    public float baseCriResist = 30f;

    // 레벨/경험치
    public int startLevel = 1;
    public int startMaxEXP = 100;
}

