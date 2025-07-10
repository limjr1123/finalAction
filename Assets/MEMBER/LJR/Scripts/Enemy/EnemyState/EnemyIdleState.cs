using UnityEngine;

public class EnemyIdleState : EnemyState<EnemyController>
{
    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.anim.SetBool("Idle", true);
    }

    public override void Execute()
    {
        Debug.Log("Enemy is in Idle State");
        enemy.target = enemy.FindTarget();
        if(enemy.target != null)
        {
            enemy.stateMachine.ChangeState(enemy.stateDict[EnemyStates.Battle]);
        }
    }

    public override void Exit()
    {
        enemy.anim.SetBool("Idle", false);
    }
}
