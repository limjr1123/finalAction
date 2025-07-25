using System;
using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    private PlayerStateMachine stateMachine;    

    public float maxHealth = 100f;
    public float currentHealth = 0;

    private bool isDead = false;

    public static event Action OnPlayerDied; // ��� �̺�Ʈ

    void Start()
    {
        currentHealth = maxHealth;
        stateMachine = GetComponent<PlayerStateMachine>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            Debug.Log("�÷��̾� �ǰ�");

            float exDamage = 10f; // �ӽ� ������ ��

            TakeDamage(exDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth > 0f)
        {
            stateMachine?.GetDamage(); // ���� �ӽſ� �ǰ� ��ȣ ������
        }
        else
        {
            PlayerDie();
        }
    }

    void PlayerDie()
    {
        isDead = true;
        currentHealth = 0f;
        Debug.Log("�÷��̾� ���");
        stateMachine?.Die();

        OnPlayerDied?.Invoke(); // ��� �̺�Ʈ ȣ��
    }

}
