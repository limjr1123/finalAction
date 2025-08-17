using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossStates
{
    Idle,
    Move,
    Run,
    Battle,
    Attack,
    GetHit,
    Dead,
}

public class BossController : MonoBehaviour
{
    public float fov = 360f; // 시야각

    public EnemyStat stats { get; private set; }
    public Dictionary<BossStates, EnemyState<BossController>> stateDict { get; private set; }
    [field: SerializeField] public EnemyStateMachine<BossController> stateMachine { get; private set; }
    public Animator anim { get; private set; }
    public bool isAttacking { get; set; } = false;
    [field: SerializeField] public GameObject target { get; set; } = null;
    public PlayerStats targetStats { get; set; } = null;
    public NavMeshAgent navAgent { get; private set; }
    public EnemyVision enemyVision { get; internal set; }

    public Collider Collider { get; set; }
    public MeleeBoss meleeBoss { get; private set; }

    //[field: SerializeField] public EnemyType enemyType { get; set; }
    public List<string> getHitAnimations { get; set; } = new List<string>();

    Vector3 prevPos;

    float moveSpeed;

    public bool inGetHit { get; set; } = false;

    void Start()
    {
        stats = GetComponent<EnemyStat>();
        anim = GetComponent<Animator>();
        enemyVision = GetComponentInChildren<EnemyVision>();
        navAgent = GetComponent<NavMeshAgent>();
        meleeBoss = GetComponent<MeleeBoss>();
        Collider = GetComponent<Collider>();

        stateDict = new Dictionary<BossStates, EnemyState<BossController>>();
        stateDict[BossStates.Idle] = GetComponent<BossIdleState>();
        stateDict[BossStates.Battle] = GetComponent<BossBattleState>();
        stateDict[BossStates.Attack] = GetComponent<BossAttackState>();
        stateDict[BossStates.GetHit] = GetComponent<BossGetHitState>();
        stateDict[BossStates.Dead] = GetComponent<BossDeadState>();

        //EnemyVision OnTargetDetected에 타겟 설정 매서드 구독
        enemyVision.OnTargetDetected += SetTarget;
        enemyVision.SetAggroRange(stats.aggroRange.GetValue()); // 어그로 범위 설정

        stateMachine = new EnemyStateMachine<BossController>(this);

        stateMachine.ChangeState(stateDict[BossStates.Idle]);

        navAgent.speed = stats.moveSpeed.GetValue(); // 초기 이동 속도 설정
        moveSpeed = stats.moveSpeed.GetValue(); // 초기 이동 속도 설정
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Execute();

        var deltaPos = transform.position - prevPos;    // 이전 위치와 현재 위치의 차이 계산
        var velocity = deltaPos / Time.deltaTime;       // 이동 속도 계산

        float forwardSpeed = Vector3.Dot(velocity, transform.forward); // 이동 방향과 속도 벡터의 내적 계산

        // magnitude로 이동속도 벡터의 크기를 가져와서 실제 설정된Speed에 맞게 비율을 계산(0~1)
        anim.SetFloat("forwardSpeed", forwardSpeed / moveSpeed, 0.2f, Time.deltaTime); // 애니메이터의 이동 속도 설정

        float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);     // 이동 방향과 현재 방향의 각도 계산
        float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
        anim.SetFloat("strafeAmount", strafeSpeed, 0.2f, Time.deltaTime); // 애니메이터의 측면 이동 속도 설정

        prevPos = transform.position; // 현재 위치 저장

        if (stats.currentHealth <= 0)
        {
            Debug.Log("Boss is dead, changing state to Dead.");
            // 애니메이션을 재시작하여 Dead 상태로 전환
            anim.enabled = false;
            anim.enabled = true;  
            stateMachine.ChangeState(stateDict[BossStates.Dead]);
        }
        //else { stateMachine.ChangeState(stateDict[BossStates.Battle]); }
    }

    // 타겟 설정 메서드(enemyVision함수에 의해 호출됨)
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        targetStats = target.GetComponent<PlayerStats>();
    }
    public void TargetChaseDirection(Vector3 _direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
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

    public void ChangeState(BossStates state)
    {
        stateMachine.ChangeState(stateDict[state]);
    }

    // State 확인 메서드
    public bool IsInState(BossStates states)
    {
        return stateMachine.currentState == stateDict[states];
    }

}
