using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "PlayerStatSetting/PlayerStatsData")]
public class PlayerStatsData : ScriptableObject
{
    [Header("int stat")]
    public int maxHealth; // �ִ� ü��
    public int maxMana; // �ִ� ����
    public int manaRegen; // ���� ȸ�� �ӵ�
    public int maxStamina; // �ִ� ���¹̳�
    public int staminaRegen; // ���¹̳� ȸ�� �ӵ�
    public int defense; // ����
    public int magicDefense; // ���� ����
    public int Str; // ��
    public int Dex; // ��ø
    public int Int; // ����
    public int attackDamage; // ���� ���ݷ�
    public int magicDamage; // ���� ���ݷ�

    [Header("float stat")]
    public float moveSpeed; // �⺻ �̵��ӵ�
    public float sprintSpeed; // �޸��� �ӵ�
    public float attackSpeed; // ���� �ӵ�
    public float criRate; // ġ��Ÿ Ȯ��
    public float criDamage; // ġ��Ÿ ���ط�
    public float criResist; // ġ��Ÿ ����

    [Header("���� ���� ����")]
    public int level; // �÷��̾� ����
    public int maxEXP; // �ִ� ����ġ
    public int currentEXP; // ���� ����ġ
}
