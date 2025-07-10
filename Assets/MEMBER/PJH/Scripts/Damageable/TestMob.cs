using UnityEngine;

public class TestMob : MonoBehaviour, IDamageable
{
    public float MaxHealth = 100f; // ������ ü��
    public float CurrentHealth; // ���� ������ ü��


    void Start()
    {
        CurrentHealth = MaxHealth; // ������ �� ü���� �ִ� ü������ �ʱ�ȭ
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage; // �������� �޾� ���� ü�� ����
        Debug.Log($"���� ü��: {CurrentHealth}");
       
    }
}
