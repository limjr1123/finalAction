using System.Collections;
using UnityEngine;

public class BossAttackState : EnemyState<BossController>
{
    bool isAttacking;
    BossController boss;

    public override void Enter(BossController owner)
    {
        boss = owner;
        boss.navAgent.stoppingDistance = boss.stats.attackRange.GetValue();
    }

    public override void Execute()
    {
        if (isAttacking)
            return;

        float distanceToPlayer = Vector3.Distance(boss.target.transform.position, transform.position);

        if (distanceToPlayer <= boss.stats.attackRange.GetValue() + 0.03f)
        {
            // 공격 범위 내에서는 플레이어 주변을 맴도는 목적지 설정
            Vector3 directionToPlayer = (boss.target.transform.position - transform.position).normalized;
            Vector3 orbitPosition = boss.target.transform.position - directionToPlayer * (boss.stats.attackRange.GetValue() - 0.1f);

            boss.navAgent.SetDestination(orbitPosition);

            // 수동 회전 처리
            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero)
            {
                float dot = Vector3.Dot(boss.transform.forward, directionToPlayer);

                if (dot < 0.98f)
                {
                    boss.TargetChaseDirection(directionToPlayer);
                }
                else
                {
                    StartCoroutine(MeleeAttack());
                }
            }
        }
        else
        {
            // 공격 범위 밖이면 직접 추적
            boss.navAgent.SetDestination(boss.target.transform.position);
        }
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        boss.anim.applyRootMotion = true;
        boss.meleeBoss.TryToAttack();
        boss.navAgent.isStopped = true;
        yield return new WaitUntil(() => boss.meleeBoss.attackState == EnemyAttackStateInfo.Idle);
        
        if (!boss.navAgent.enabled)
            yield return null;
        boss.anim.applyRootMotion = false;
        boss.navAgent.isStopped = false;
        isAttacking = false;

        if (boss.IsInState(BossStates.Attack))
            boss.ChangeState(BossStates.Battle);
    }
}
