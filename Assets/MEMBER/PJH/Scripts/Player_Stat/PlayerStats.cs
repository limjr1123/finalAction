using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatsData baseStats;

    [Header("�������")]
    public int currentHealth; // ���� ü��
    public int currentMana; // ���� ����
    public int currentStamina; // ���� ���¹̳�
    public int level; // �÷��̾� ����
    public int currentEXP; // ���� ����ġ

    [Header("�⺻ ����")]
    public Stat maxHealth; // �ִ� ü��
    public Stat maxMana; // �ִ� ����
    public Stat manaRegen; // ���� ȸ�� �ӵ�
    public Stat maxStamina; // �ִ� ���¹̳�
    public Stat staminaRegen; // ���¹̳� ȸ�� �ӵ�
    public Stat defense; // ����
    public Stat magicDefense; // ���� ����
    public Stat Str; // ��
    public Stat Dex; // ��ø
    public Stat Int; // ����

    [Header("�̵� ���� ����")]
    public FloatStat moveSpeed; // �⺻ �̵��ӵ�
    public FloatStat sprintSpeed; // �޸��� �ӵ�

    [Header("���� ���� ����")]
    public Stat attackDamage; // ���� ���ݷ�
    public Stat magicDamage; // ���� ���ݷ�
    public FloatStat attackSpeed; // ���� �ӵ�

    [Header("ġ��Ÿ ���� ����")]
    public FloatStat criRate; // ġ��Ÿ Ȯ��
    public FloatStat criDamage; // ġ��Ÿ ���ط�
    public FloatStat criResist; // ġ��Ÿ ����

    void Awake()
    {
        ApplyBaseStats(); // ���� �ʱ�ȭ
    }

    public void ApplyBaseStats()
    {
        if (baseStats == null)
        {
            Debug.LogError("Base Stats ������ �ʿ�");
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
