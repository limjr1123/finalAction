using System.Collections;
using UnityEngine;

public class HealingCircle : MonoBehaviour
{
    public string SkillName { get; private set; } = "HealingCircle"; // 스킬 이름

    [SerializeField] public int Amount { get; private set; } // 회복량
    [SerializeField] public float Radius { get; private set; }  // 회복 범위
    [SerializeField] public float Duration { get; private set; }  // 회복 지속 시간
    [SerializeField] public float HealPeriod { get; private set; } = 1f; // 회복 주기

    ParticleSystem healingCircleEffect; // Healing Circle 이펙트

    Collider healingCircleCollider; // Healing Circle 콜라이더

    bool isSkillStart = false; // 스킬 시작 여부

    void Update()
    {
        if (isSkillStart)
            StartCoroutine(HealCoroutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 플레이어에게 회복 효과 적용
            EnemyStat enemy = other.GetComponent<EnemyStat>();
            if (enemy != null)
            {
                enemy.IncreaseHealth(Amount);
            }
        }
    }

    private IEnumerator HealCoroutine()
    {
        isSkillStart = false;

        for (int i = 0; i < 5; i++)
        {
            healingCircleCollider.enabled = true; // Healing Circle 콜라이더 활성화
            yield return new WaitForSeconds(HealPeriod); // Duration 동안 기다림
            healingCircleCollider.enabled = false; // Healing Circle 콜라이더 비활성화
        }
    }

    public void Initialize(int amount, float radius, float duration)
    {
        Amount = amount;
        Radius = radius;
        Duration = duration;
        // Healing Circle 이펙트와 콜라이더 초기화
        healingCircleEffect = GetComponent<ParticleSystem>();
        healingCircleCollider = GetComponent<Collider>();

        var main = healingCircleEffect.main;
        main.duration = Duration;
        main.startSize = Radius;

        isSkillStart = true;
    }

    public string GetSkillName()
    {
        return SkillName;
    }
}