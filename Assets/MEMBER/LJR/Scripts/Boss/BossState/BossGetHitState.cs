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
        //StartCoroutine(GettingHitAnim());
    }
    public override void Execute()
    {
        if (boss.stats.currentHealth <= 0)
        {
            Debug.Log("Boss is dead, changing state to Dead.");
            boss.stateMachine.ChangeState(boss.stateDict[BossStates.Dead]);
        }
        else { boss.stateMachine.ChangeState(boss.stateDict[BossStates.Battle]); }
    }

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
