using UnityEngine;

public class BossDeadState : EnemyState<BossController>
{
    BossController boss;

    public override void Enter(BossController owner)
    {
        boss = owner;
        boss.navAgent.ResetPath();
    }

    public override void Execute()
    {
        if (boss.stats.currentHealth <= 0)
        {
            boss.anim.CrossFade("Dead", 0.2f); // "Dead" 애니메이션으로 전환

            boss.navAgent.enabled = false; // NavMeshAgent 비활성화
            boss.enabled = false; // EnemyController 비활성화
            boss.enemyVision.enabled = false; // EnemyVision 비활성화
            boss.inGetHit = false; // GetHit 상태 해제

            // 적이 죽었을 때 플레이어에게 경험치 추가
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.AddExp(500);
            }
        }
    }
}
