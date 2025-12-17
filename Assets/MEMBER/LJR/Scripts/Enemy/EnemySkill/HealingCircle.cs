using System.Collections;
using System.Collections.Generic;
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

    private HashSet<EnemyStat> targetsInRange = new HashSet<EnemyStat>();

    float timer = 0f;

    private void Update()
    {
        if (!isSkillStart) return;

        timer += Time.deltaTime;

        if (timer >= HealPeriod)
        {
            foreach (var target in targetsInRange)
            {
                if (target != null)
                    target.IncreaseHealth(Amount);
            }
            timer -= HealPeriod;  // 또는 timer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var stat = other.GetComponent<EnemyStat>();
            if (stat != null) targetsInRange.Add(stat);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var stat = other.GetComponent<EnemyStat>();
            if (stat != null) targetsInRange.Remove(stat);
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
        //main.duration = Duration;
        main.startSize = Radius;

        isSkillStart = true;
    }

    public string GetSkillName()
    {
        return SkillName;
    }
}