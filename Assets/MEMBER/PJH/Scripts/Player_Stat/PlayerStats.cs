using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("�⺻ ����")]
    public Stat maxHealth; // �ִ� ü��
    public Stat currentHealth; // ���� ü��

    public Stat maxMana; // �ִ� ����
    public Stat currentMana; // ���� ����
    public Stat manaRegen; // ���� ȸ�� �ӵ�

    public Stat maxStamina; // �ִ� ���¹̳�
    public Stat currentStamina; // ���� ���¹̳�
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

    [Header("���� ���� ����")]
    public Stat level; // �÷��̾� ����
    public Stat maxEXP; // �ִ� ����ġ
    public Stat currentEXP; // ���� ����ġ
}
