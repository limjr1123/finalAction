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
        enemy.navAgent.SetDestination(enemy.target.transform.position);

        if (Vector3.Distance(enemy.target.transform.position, enemy.transform.position) <= enemy.stats.attackRange.GetValue() + 0.03f)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        enemy.anim.applyRootMotion = true;
        enemy.meleeEnemy.TryToAttack();

        yield return new WaitUntil(() => enemy.meleeEnemy.attackState == EnemyAttackStateInfo.Idle);

        enemy.anim.applyRootMotion = false;
        isAttacking = false;

        if (enemy.IsInState(EnemyStates.Attack))
            enemy.ChangeState(EnemyStates.Battle);
    }

    public override void Exit()
    {
        enemy.navAgent.ResetPath();
    }
}
