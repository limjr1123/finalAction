using System;
using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    private PlayerStateMachine stateMachine;    

    public float maxHealth = 100f;
    public float currentHealth = 0;

    private bool isDead = false;

    public static event Action OnPlayerDied; // 사망 이벤트

    void Start()
    {
        currentHealth = maxHealth;
        stateMachine = GetComponent<PlayerStateMachine>();
        //InvokeRepeating(nameof(DamageOverTime), 1f, 1f); // 실험용 필요없으면 각주처리
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            Debug.Log("플레이어 피격");

            float exDamage = 10f; // 임시 데미지 값

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
            stateMachine?.GetDamage(); // 상태 머신에 피격 신호 보내기
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        currentHealth = 0f;
        Debug.Log("플레이어 사망");
        stateMachine?.Die();

        OnPlayerDied?.Invoke(); // 사망 이벤트 호출
    }

    //void DamageOverTime() // 사망 실험용
    //{
    //    TakeDamage(10f);
    //}

}
