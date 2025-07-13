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
    getHit,
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

    Vector3 prevPos;

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
        stateDict[EnemyStates.getHit] = GetComponent<EnemyGetHitState>();

        stateMachine = new EnemyStateMachine<EnemyController>(this);

        // Idle 상태로 시작
        stateMachine.ChangeState(stateDict[EnemyStates.Idle]);
    }

    void Update()
    {
        stateMachine.Execute();

        var deltaPos = transform.position - prevPos;    // 이전 위치와 현재 위치의 차이 계산
        var velocity = deltaPos / Time.deltaTime;       // 이동 속도 계산

        float forwardSpeed = Vector3.Dot(velocity, transform.forward); // 이동 방향과 속도 벡터의 내적 계산

        // magnitude로 이동속도 벡터의 크기를 가져와서 실제 설정된Speed에 맞게 비율을 계산(0~1)
        anim.SetFloat("forwardSpeed", forwardSpeed / navAgent.speed, 0.2f, Time.deltaTime); // 애니메이터의 이동 속도 설정
        prevPos = transform.position; // 현재 위치 저장
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

    // State 확인 메서드
    public bool IsInState(EnemyStates states)
    {
        return stateMachine.currentState == stateDict[states];
    }
}
