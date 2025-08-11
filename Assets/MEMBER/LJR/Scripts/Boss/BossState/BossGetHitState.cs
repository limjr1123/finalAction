using System.Collections;
using UnityEngine;

public class BossGetHitState : EnemyState<BossController>
{
    BossController boss;
    [SerializeField] int hitAnimationIndex = 0;

    public override void Enter(BossController owner)
    {
        boss = owner;
        hitAnimationIndex = boss.getHitAnimations.Count;
        StartCoroutine(GettingHitAnim());
    }


    // Update is called once per frame
    IEnumerator GettingHitAnim()
    {
        boss.inGetHit = true;
        string selectedAnim = boss.getHitAnimations[Random.Range(0, hitAnimationIndex)];
        boss.anim.CrossFade(selectedAnim, 0.2f);

        yield return new WaitForSeconds(boss.anim.GetCurrentAnimatorStateInfo(1).length);
        boss.inGetHit = false;
        boss.stateMachine.ChangeState(boss.stateDict[BossStates.Battle]);
    }
}
