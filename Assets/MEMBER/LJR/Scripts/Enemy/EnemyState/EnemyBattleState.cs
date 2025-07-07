using UnityEngine;

public class EnemyBattleState : EnemyState<EnemyController>
{
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.anim.SetBool("Battle", true);
    }

    public override void Execute()
    {
    }
}