using System;
using UnityEngine;

[System.Serializable]
public struct PlayerDamageRange
{
    public int min;
    public int max;

    public PlayerDamageRange(int damage, float ratio)
    {
        min = Mathf.RoundToInt(damage * (1f - ratio));
        max = Mathf.RoundToInt(damage * (1f + ratio));
    }

    // ������ �������� ������ ���� ��ȯ(ũ��Ƽ�� Ȯ���� ������� �ʴ� ��� ��� ����)
    public int GetRandomDamage()
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    // ġ��Ÿ Ȯ���� ġ��Ÿ ���ط� ������ �����Ͽ� ���� ������ ���� ��ȯ
    public int CalculateDamage(float criticalChance, float criticalDamageRatio)
    {
        int baseDamage = GetRandomDamage();
        bool isCritical = UnityEngine.Random.Range(0f, 1f) < criticalChance;

        if (isCritical)
        {
            int damage = Mathf.RoundToInt(baseDamage * criticalDamageRatio);
            return damage;
        }
        else
        {
            return baseDamage;
        }
    }
}

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

    public PlayerDamageRange attackDamageRange;   // ���� ���ط� ����
    public float damageRange = 0.2f;        // ���� ���ط� ���� ���� (20%)


    public Vector3 currentPos;


    protected Action OnHealthChanged;

    void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        ApplyBaseStats(); // ���� �ʱ�ȭ
        OnHealthChanged += () => HealthCheck();
    }

    public int GetAttackDamage()
    {
        var currentDamageRange = new PlayerDamageRange(attackDamage.GetValue(), damageRange);
        return currentDamageRange.CalculateDamage(criRate.GetValue(), criDamage.GetValue());
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
        if (other.CompareTag("HitBox"))
        {
            Debug.Log("�÷��̾� �ǰ�");
        }
    }

    public void TakePhysicalDamage(int damage)
    {
        if (isDead) return; // �̹� ����� ��� ����
        int finalDamage = CheckTargetArmor(this, damage);

        DecreaseHealth(finalDamage);

        OnHealthChanged?.Invoke();
        Debug.Log($"�÷��̾ {finalDamage}�� ���� ���ظ� �޾ҽ��ϴ�. ���� ü��: {currentHealth}");
    }


    public void TakeMagicalDamage(int damage)
    {
        if (isDead) return; // �̹� ����� ��� ����
        int finalDamage = CheckTargetMagicArmor(this, damage);

        DecreaseHealth(finalDamage);

        OnHealthChanged?.Invoke();
        Debug.Log($"�÷��̾ {finalDamage}�� ���� ���ظ� �޾ҽ��ϴ�. ���� ü��: {currentHealth}");
    }

    private void HealthCheck()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual int DecreaseHealth(int finalDamage)
    {
        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        return currentHealth;
    }

    protected virtual int CheckTargetArmor(PlayerStats target, int _damage)
    {
        // ���¿� ���� ���� ���� ����
        int reducedDamage = _damage - target.defense.GetValue();
        return Mathf.Max(reducedDamage, 1); // ���ذ� 1 ���Ϸ� �������� �ʵ��� ����
    }

    protected virtual int CheckTargetMagicArmor(PlayerStats target, int _damage)
    {
        // ���¿� ���� ���� ���� ����
        int reducedDamage = _damage - target.magicDefense.GetValue();
        return Mathf.Max(reducedDamage, 1); // ���ذ� 1 ���Ϸ� �������� �ʵ��� ����
    }

    private void Die()
    {
        if (isDead) return; // �ߺ� ���� ����

        isDead = true;
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

        // ����� ��ġ�� �ű��
        transform.position = data.savePos;
        Debug.Log($"������ ���� �Ϸ�.");
    }
}
