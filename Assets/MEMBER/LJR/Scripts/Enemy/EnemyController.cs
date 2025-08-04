using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// EnemyType : 적의 종류를 정의하는 열거형
public enum EnemyType
{
    Melee,  // 근접 적
    Range,  // 원거리 적
}

// EnemyStates : 적의 상태를 정의하는 열거형
public enum EnemyStates
{
    Idle, Move, Run, Battle, Attack, GetHit, Dead,
}

// EnemyAttackStateInfo : 적의 공격 상태를 정의하는 열거형
public enum EnemyAttackStateInfo
{
    Idle,           // 대기 상태
    Windup,         // 공격 준비(선딜레이)
    Impact,         // 타격 판정 구간
    AttackDelay     // 후딜레이
}

public class EnemyController : MonoBehaviour
{
    public float fov = 180f; // 시야각
    public EnemyStat stats { get; private set; }
    public Dictionary<EnemyStates, EnemyState<EnemyController>> stateDict { get; private set; }
    public EnemyStateMachine<EnemyController> stateMachine { get; private set; }
    public Animator anim { get; private set; }
    public bool isAttacking { get; set; } = false;
    [field: SerializeField] public GameObject target { get; set; } = null;
    public NavMeshAgent navAgent { get; private set; }
    public EnemyVision enemyVision { get; internal set; }
    public MeleeEnemy meleeEnemy { get; private set; }
    public RangeEnemy rangeEnemy { get; private set; }
    [field: SerializeField] public EnemyType enemyType { get; set; }
    public List<string> getHitAnimations { get; set; } = new List<string>();

    Vector3 prevPos;

    private void Awake()
    {
        // enemy Type 할당
        if (GetComponent<MeleeEnemy>())
        {
            meleeEnemy = GetComponent<MeleeEnemy>();
            enemyType = EnemyType.Melee;
        }
        else
        {
            rangeEnemy = GetComponent<RangeEnemy>();
            enemyType = EnemyType.Range;
        }
    }

    void Start()
    {
        stats = GetComponent<EnemyStat>();
        anim = GetComponent<Animator>();
        enemyVision = GetComponentInChildren<EnemyVision>();
        navAgent = GetComponent<NavMeshAgent>();    // NavMeshAgent 컴포넌트 가져오기

        stateDict = new Dictionary<EnemyStates, EnemyState<EnemyController>>();
        stateDict[EnemyStates.Idle] = GetComponent<EnemyIdleState>();
        stateDict[EnemyStates.Battle] = GetComponent<EnemyBattleState>();
        stateDict[EnemyStates.Attack] = GetComponent<EnemyAttackState>();
        stateDict[EnemyStates.Dead] = GetComponent<EnemyDeadState>();
        stateDict[EnemyStates.GetHit] = GetComponent<EnemyGetHitState>();
        
        //EnemyVision OnTargetDetected에 타겟 설정 매서드 구독
        enemyVision.OnTargetDetected += SetTarget;
        enemyVision.SetAggroRange(stats.aggroRange.GetValue()); // 어그로 범위 설정

        stateMachine = new EnemyStateMachine<EnemyController>(this);
        // Idle 상태로 시작
        stateMachine.ChangeState(stateDict[EnemyStates.Idle]);
        CacheGetHitAnimations();

        navAgent.speed = stats.moveSpeed.GetValue(); // 초기 이동 속도 설정
    }

    void Update()
    {
        stateMachine.Execute();

        var deltaPos = transform.position - prevPos;    // 이전 위치와 현재 위치의 차이 계산
        var velocity = deltaPos / Time.deltaTime;       // 이동 속도 계산

        float forwardSpeed = Vector3.Dot(velocity, transform.forward); // 이동 방향과 속도 벡터의 내적 계산

        // magnitude로 이동속도 벡터의 크기를 가져와서 실제 설정된Speed에 맞게 비율을 계산(0~1)
        anim.SetFloat("forwardSpeed", forwardSpeed / navAgent.speed, 0.2f, Time.deltaTime); // 애니메이터의 이동 속도 설정

        float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);     // 이동 방향과 현재 방향의 각도 계산


        float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);


        if (target && Vector3.Distance(target.transform.position, transform.position) < stats.attackRange.GetValue())
            strafeSpeed = 0; // 타겟과의 거리가 공격 범위 내에 있으면 측면 이동 속도 0으로 설정

        anim.SetFloat("strafeAmount", strafeSpeed, 0.2f, Time.deltaTime); // 애니메이터의 측면 이동 속도 설정

        prevPos = transform.position; // 현재 위치 저장
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }


    // GetHit 애니메이션을 캐시하는 메서드.
    void CacheGetHitAnimations()
    {
        RuntimeAnimatorController controller = anim.runtimeAnimatorController;
        if (controller != null)
        {
            foreach (AnimationClip clip in controller.animationClips)
            {
                if (clip.name.StartsWith("GetHit"))
                {
                    getHitAnimations.Add(clip.name);
                }
            }
        }
    }

    // HitBox와 충돌했을 때 호출되는 메서드.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            Debug.Log("타격 성공");
            stateMachine.ChangeState(stateDict[EnemyStates.GetHit]);
        }
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
