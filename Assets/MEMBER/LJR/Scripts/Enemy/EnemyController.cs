using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

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
    public float fov = 180f; // 시야각
    public EnemyStat stats { get; private set; }
    public Dictionary<EnemyStates, EnemyState<EnemyController>> stateDict { get; private set; }
    public EnemyStateMachine<EnemyController> stateMachine { get; private set; }
    public Animator anim { get; private set; }
    public bool isAttacking { get; set; } = false;
    [field:SerializeField] public GameObject target { get; set; } = null;
    public NavMeshAgent navAgent { get; private set; }
    public EnemyVision visionSensor { get; internal set; }
    public MeleeEnemy meleeEnemy { get; private set; }

    public float battleMovementTimer { get; set; } = 0f;

    void Start()
    {
        stats = GetComponent<EnemyStat>();
        anim = GetComponent<Animator>();
        visionSensor = GetComponentInChildren<EnemyVision>();
        navAgent = GetComponent<NavMeshAgent>();    // NavMeshAgent 컴포넌트 가져오기
        meleeEnemy = GetComponent<MeleeEnemy>();

        stateDict = new Dictionary<EnemyStates, EnemyState<EnemyController>>();
        stateDict[EnemyStates.Idle] = GetComponent<EnemyIdleState>();
        stateDict[EnemyStates.Battle] = GetComponent<EnemyBattleState>();
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


    public GameObject FindTarget()
    {
        if (target == null)
            return null;

        var vecToTarget = target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, vecToTarget);

        if (angle <= fov / 2)
        {
            return target; // 시야 내에 있는 타겟 반환
        }
        return null; // 시야 밖에 있는 경우 null 반환
    }

    public void ChangeState(EnemyStates state)
    {
        stateMachine.ChangeState(stateDict[state]);
    }
    public bool IsInState(EnemyStates states)
    {
        return stateMachine.currentState == stateDict[states];
    }
}
