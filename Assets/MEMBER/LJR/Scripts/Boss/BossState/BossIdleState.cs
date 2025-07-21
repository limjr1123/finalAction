using UnityEngine;

public class BossIdleState : EnemyState<BossController>
{
    BossController boss;

    public override void Enter(BossController owner)
    {
        boss = owner;
    }

    public override void Execute()
    {
        Debug.Log("Boss is in Idle State");
        boss.target = boss.FindTarget();
        if (boss.target != null)
        {
            boss.stateMachine.ChangeState(boss.stateDict[BossStates.Battle]);
        }
    }

    public override void Exit()
    {

    }
}
