using System.Collections;
using UnityEngine;

public class EnemyGetHitState : EnemyState<EnemyController>
{
    EnemyController enemy;
    [SerializeField]int hitAnimationIndex = 0;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        hitAnimationIndex = enemy.getHitAnimations.Count;
        StartCoroutine(GettingHitAnim());
    }

    // 공격을 받았을 때 호출되는 애니메이션 코루틴.
    IEnumerator GettingHitAnim()
    {
        enemy.inGetHit = true;
        string selectedAnim = enemy.getHitAnimations[Random.Range(0, hitAnimationIndex)];
        enemy.anim.CrossFade(selectedAnim, 0.2f);

        yield return new WaitForSeconds(enemy.anim.GetCurrentAnimatorStateInfo(1).length);
        enemy.inGetHit = false;
        enemy.stateMachine.ChangeState(enemy.stateDict[EnemyStates.Battle]);
    }
}
