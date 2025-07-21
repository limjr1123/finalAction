using System.Collections;
using UnityEngine;

public class EnemyAttackState : EnemyState<EnemyController>
{
    bool isAttacking;
    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.navAgent.stoppingDistance = enemy.stats.attackRange.GetValue();
    }

    public override void Execute()
    {
        if (isAttacking)
            return;

        float distanceToPlayer = Vector3.Distance(enemy.target.transform.position, transform.position);

        if (distanceToPlayer <= enemy.stats.attackRange.GetValue() + 0.03f)
        {
            // 공격 범위 내에서는 플레이어 주변을 맴도는 목적지 설정
            Vector3 directionToPlayer = (enemy.target.transform.position - transform.position).normalized;
            Vector3 orbitPosition = enemy.target.transform.position - directionToPlayer * (enemy.stats.attackRange.GetValue() - 0.1f);

            enemy.navAgent.SetDestination(orbitPosition);

            // 수동 회전 처리
            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero)
            {
                float dot = Vector3.Dot(enemy.transform.forward, directionToPlayer);

                if (dot < 0.9f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
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
            enemy.navAgent.SetDestination(enemy.target.transform.position);
        }
    }

    IEnumerator MeleeAttack()
    {        
        isAttacking = true;
        enemy.anim.applyRootMotion = true;
        enemy.meleeEnemy.TryToAttack();
        enemy.navAgent.isStopped = true;
        yield return new WaitUntil(() => enemy.meleeEnemy.attackState == EnemyAttackStateInfo.Idle);

        enemy.anim.applyRootMotion = false;
        enemy.navAgent.isStopped = false;
        isAttacking = false;

        if (enemy.IsInState(EnemyStates.Attack))
            enemy.ChangeState(EnemyStates.Battle);
    }

    public override void Exit()
    {
        enemy.navAgent.ResetPath();
    }
}
