using UnityEngine;

public class EnemyDeadState : EnemyState<EnemyController>
{
    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.StopAllCoroutines();
        enemy.navAgent.ResetPath();
    }

    public override void Execute()
    {
        if (enemy.stats.currentHealth <= 0)
        {
            enemy.anim.CrossFade("Dead", 0.2f); // "Dead" 애니메이션으로 전환

            enemy.navAgent.enabled = false; // NavMeshAgent 비활성화
            enemy.enemyVision.enabled = false; // EnemyVision 비활성화
            enemy.inGetHit = false; // GetHit 상태 해제
            enemy.collider.enabled = false; // Collider 비활성화
            enemy.enabled = false; // EnemyController 비활성화

            // 적이 죽었을 때 플레이어에게 경험치 추가
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.AddExp(100);
            }
        }
    }
}
