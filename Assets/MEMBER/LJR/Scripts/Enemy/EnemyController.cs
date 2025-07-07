using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates
{
    Idle,
    Move,
    Run,
    Battle,
    Attack,
    gettingHit,
    Dead,
}

public class EnemyController : MonoBehaviour
{
    public Stat stats;

    public Dictionary<EnemyStates, EnemyState<EnemyController>> stateDict;

    public EnemyStateMachine<EnemyController> stateMachine { get; private set; }

    public Animator anim { get; private set; }

    void Start()
    {
        anim = GetComponent<Animator>();

        stateDict = new Dictionary<EnemyStates, EnemyState<EnemyController>>();
        stateDict[EnemyStates.Idle] = GetComponent<EnemyIdleState>();
        stateDict[EnemyStates.Battle] = GetComponent<EnemyBattleState>();
        stateDict[EnemyStates.Attack] = GetComponent<EnemyAttackState>();
        stateDict[EnemyStates.Attack] = GetComponent<EnemyAttackState>();
        stateDict[EnemyStates.Dead] = GetComponent<EnemyDeadState>();

        stateMachine = new EnemyStateMachine<EnemyController>(this);

        // Idle 상태로 시작
        stateMachine.ChangeState(stateDict[EnemyStates.Idle]);
    }

    void Update()
    {
        stateMachine.Execute();
    }


    public void FindTarget()
    {

    }

    public void ChangeState(EnemyStates state)
    {
        stateMachine.ChangeState(stateDict[state]);
    }

}
