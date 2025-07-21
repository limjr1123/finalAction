using System.Collections;
using UnityEngine;

public class EnemyGetHitState : EnemyState<EnemyController>
{
    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        StartCoroutine(GettingHitAnim());
    }

    // 공격을 받았을 때 호출되는 애니메이션 코루틴.
    IEnumerator GettingHitAnim()
    {
        enemy.meleeEnemy.inGetHit = true;
        enemy.anim.CrossFade("GetHit", 0.2f);
        yield return new WaitForSeconds(enemy.anim.GetCurrentAnimatorStateInfo(1).length);
        enemy.meleeEnemy.inGetHit = false;
        enemy.stateMachine.ChangeState(enemy.stateDict[EnemyStates.Battle]);
    }
}
