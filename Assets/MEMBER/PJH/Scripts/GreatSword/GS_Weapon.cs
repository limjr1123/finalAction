using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class GS_Weapon : MonoBehaviour
{
    private PlayerStats playerStats;
    private PlayerStateMachine stateMachine;

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
                MonsterHPUI.Instance.SetTarget(enemyStat);
                MonsterHPUI.Instance.UpdateHP(enemyStat.currentHealth, enemyStat.maxHealth.GetValue());
                HitStop.Instance.StopTime();
            }
        }
    }
}
