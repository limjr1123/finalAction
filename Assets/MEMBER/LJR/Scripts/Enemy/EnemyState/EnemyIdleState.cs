using UnityEngine;

public class EnemyIdleState : EnemyState<EnemyController>
{
    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
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

    }
}
