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
    public Stat moveSpeed; // �⺻ �̵��ӵ�
    public Stat sprintSpeed; // �޸��� �ӵ�

    [Header("���� ���� ����")]
    public Stat attackDamage; // ���� ���ݷ�
    public Stat magicDamage; // ���� ���ݷ�
    public Stat attackSpeed; // ���� �ӵ�

    [Header("ġ��Ÿ ���� ����")]
    public Stat criRate; // ġ��Ÿ Ȯ��
    public Stat criDamage; // ġ��Ÿ ���ط�
    public Stat criResist; // ġ��Ÿ ����

    [Header("���� ���� ����")]
    public Stat level; // �÷��̾� ����
    public Stat maxEXP; // �ִ� ����ġ
    public Stat currentEXP; // ���� ����ġ
}
