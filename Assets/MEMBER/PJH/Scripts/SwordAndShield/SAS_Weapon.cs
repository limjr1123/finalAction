using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class SAS_Weapon : MonoBehaviour
{
    private PlayerStats playerStats;
    private PlayerStateMachine stateMachine;
    [SerializeField] private HitEffectType hitEffectType;

    private void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();
        stateMachine = GetComponentInParent<PlayerStateMachine>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            int finalDamage = playerStats.GetAttackDamage();

            if (other.TryGetComponent(out EnemyStat enemyStat))
            {
                enemyStat.TakeAttackDamage(finalDamage);
                HitEffectManager.Instance.EffectCreate(other.transform, hitEffectType, new Vector3(0, 1f, 0));
                MonsterHPUI.Instance.SetTarget(enemyStat);
                MonsterHPUI.Instance.UpdateHP(enemyStat.currentHealth, enemyStat.maxHealth.GetValue());
                HitStop.Instance.StopTime();
            }
        }
    }
}
