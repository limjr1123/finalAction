using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] 
    private PlayerStatsData baseStats;
    public static event Action OnPlayerDied; // �÷��̾� ��� �̺�Ʈ
    private PlayerStateMachine stateMachine;
    private bool isDead = false;

    public string characterName;
    public JobData jobData;

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
    public Stat maxEXP; // �ִ� ����ġ
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
        stateMachine = GetComponent<PlayerStateMachine>();
        ApplyBaseStats(); // ���� �ʱ�ȭ
    }

    public void ApplyBaseStats()
    {
        if (baseStats == null)
        {
            Debug.LogError("Base Stats ������ �ʿ�");
            return;
        }
        level = 1;
        maxEXP.SetDefaultValue(baseStats.maxEXP);
        currentEXP = 0;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox")) // �±״� ���� ����ϴ� ������ ����
        {
            Debug.Log("�÷��̾� �ǰ�");

            int exDamage = 20;

            TakeDamage(exDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        // ������ ������ ���� ������ ��� (�ּ� 1�� �������� �޵��� ����)
        currentHealth -= damage;
        Debug.Log($"{damage}�� �������� �Ծ����ϴ�. ���� ü��: {currentHealth}");

        if (currentHealth > 0)
        {
            // ���� ����ִٸ� �ǰ� ���·� ��ȯ
            stateMachine?.GetDamage();
        }
        else
        {
            // ü���� 0 ���ϸ� ��� ó��
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        if (isDead) return; // �ߺ� ���� ����

        isDead = true;
        currentHealth = 0;
        stateMachine?.Die();
        OnPlayerDied?.Invoke(); // ����̺�Ʈ ȣ��
    }

    public void LoadData(PlayerSaveData data)
    {
        if (data == null)
        {
            return;
        }
        this.characterName = data.characterName;
        this.jobData = data.jobData;

        this.level = data.level;
        this.currentEXP = data.currentEXP;
        this.currentHealth = data.currentHealth;
        this.currentMana = data.currentMana;
        this.currentStamina = data.currentStamina;

        maxHealth.SetDefaultValue(data.maxHealth);
        maxMana.SetDefaultValue(data.maxMana);
        manaRegen.SetDefaultValue(data.manaRegen);
        maxStamina.SetDefaultValue(data.maxStamina);
        staminaRegen.SetDefaultValue(data.staminaRegen);
        maxEXP.SetDefaultValue(data.maxEXP);
        defense.SetDefaultValue(data.defense);
        magicDefense.SetDefaultValue(data.magicDefense);
        Str.SetDefaultValue(data.Str);
        Dex.SetDefaultValue(data.Dex);
        Int.SetDefaultValue(data.Int);
        moveSpeed.SetDefaultValue(data.moveSpeed);
        sprintSpeed.SetDefaultValue(data.sprintSpeed);
        attackDamage.SetDefaultValue(data.attackDamage);
        magicDamage.SetDefaultValue(data.magicDamage);
        attackSpeed.SetDefaultValue(data.attackSpeed);
        criRate.SetDefaultValue(data.criRate);
        criDamage.SetDefaultValue(data.criDamage);
        criResist.SetDefaultValue(data.criResist);

        Debug.Log($"������ ���� �Ϸ�.");
    }
}
