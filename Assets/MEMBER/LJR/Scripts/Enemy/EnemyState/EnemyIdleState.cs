using UnityEngine;

public class EnemyIdleState : EnemyState<EnemyController>
{
    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.anim.SetBool("BattleState", false);
    }

    public override void Execute()
    {
        enemy.target = enemy.FindTarget();
        if(enemy.target != null)
        {
            enemy.stateMachine.ChangeState(enemy.stateDict[EnemyStates.Battle]);
        }
    }

    public override void Exit()
    {

    }

    public void RandomMovePattern()
    {

    }
}
