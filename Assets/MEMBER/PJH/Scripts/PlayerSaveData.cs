using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public string characterName;
    public JobData jobData;

    // ���� ����
    public int currentHealth;
    public int currentMana;
    public int currentStamina;
    public int level;
    public int currentEXP;

    // �⺻ ����
    public int maxHealth;
    public int maxMana;
    public int manaRegen;
    public int maxStamina;
    public int staminaRegen;
    public int maxEXP;
    public int defense;
    public int magicDefense;
    public int Str;
    public int Dex;
    public int Int;

    // �̵� ���� ����
    public float moveSpeed;
    public float sprintSpeed;

    // ���� ���� ����
    public int attackDamage;
    public int magicDamage;
    public float attackSpeed;

    // ġ��Ÿ ���� ����
    public float criRate;
    public float criDamage;
    public float criResist;

    public PlayerSaveData() { }

    public PlayerSaveData(PlayerStats playerStats)
    {
        characterName = playerStats.characterName;
        jobData = playerStats.jobData;

        // ���� ���� ������ ���� ����
        currentHealth = playerStats.currentHealth;
        currentMana = playerStats.currentMana;
        currentStamina = playerStats.currentStamina;
        level = playerStats.level;
        currentEXP = playerStats.currentEXP;

        // Stat �Ǵ� FloatStat Ÿ�� �������� GetValue() ���� ������
        maxHealth = playerStats.maxHealth.GetValue();
        maxMana = playerStats.maxMana.GetValue();
        manaRegen = playerStats.manaRegen.GetValue();
        maxStamina = playerStats.maxStamina.GetValue();
        staminaRegen = playerStats.staminaRegen.GetValue();
        maxEXP = playerStats.maxEXP.GetValue();
        defense = playerStats.defense.GetValue();
        magicDefense = playerStats.magicDefense.GetValue();
        Str = playerStats.Str.GetValue();
        Dex = playerStats.Dex.GetValue();
        Int = playerStats.Int.GetValue();

        moveSpeed = playerStats.moveSpeed.GetValue();
        sprintSpeed = playerStats.sprintSpeed.GetValue();

        attackDamage = playerStats.attackDamage.GetValue();
        magicDamage = playerStats.magicDamage.GetValue();
        attackSpeed = playerStats.attackSpeed.GetValue();

        criRate = playerStats.criRate.GetValue();
        criDamage = playerStats.criDamage.GetValue();
        criResist = playerStats.criResist.GetValue();

        // ���̳� ��ġ ���� ������ ���� ����

    }
}
