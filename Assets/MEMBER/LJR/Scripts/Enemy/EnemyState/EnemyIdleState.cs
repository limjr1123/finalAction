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
        
    }
}
