using UnityEngine;

public class TestMob : MonoBehaviour, IDamageable
{
    public float MaxHealth = 100f; // 몬스터의 체력
    public float CurrentHealth; // 현재 몬스터의 체력


    void Start()
    {
        CurrentHealth = MaxHealth; // 시작할 때 체력을 최대 체력으로 초기화
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage; // 데미지를 받아 현재 체력 감소
        Debug.Log($"현재 체력: {CurrentHealth}");
       
    }
}
