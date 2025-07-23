using UnityEngine;

public class SAS_Weapon : MonoBehaviour
{
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Enemy"))
        {      
            int finalDamage = playerStats.GetAttackDamage();

            if (other.TryGetComponent(out EnemyStat enemyStat))
            {
                enemyStat.TakeAttackDamage(finalDamage);
            }
        }
    }
}
