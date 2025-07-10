using UnityEngine;

public class EnemyAttackState : EnemyState<EnemyController>
{
    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
    }

    public override void Execute()
    {
        // 공격 애니메이션이 끝나면 Idle 상태로 전환
        if (enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            enemy.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            enemy.ChangeState(EnemyStates.Idle);
        }
    }
}
