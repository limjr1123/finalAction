using System;
using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private bool isDead = false;

    public static event Action OnPlayerDied; // 사망 이벤트

    void Start()
    {
        maxHealth = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;
        currentHealth = 0f;
        Debug.Log("플레이어 사망");

        OnPlayerDied?.Invoke(); // 사망 이벤트 호출
    }


}
