using UnityEngine;

public class BossBattleState : EnemyState<BossController>
{
    BossController boss;
    AIBattleState state;

    [SerializeField] float distanceToStand;        // 타겟과의 기본 유지 거리
    [SerializeField] float adjustDistanceThreshold = 0.1f;    // 거리 조정 허용 오차

    float timer = 0;

    public override void Enter(BossController owner)
    {
        boss = owner;
        distanceToStand = boss.stats.attackRange.GetValue(); // 공격 범위에 따라 거리 설정
        boss.navAgent.stoppingDistance = distanceToStand; // NavMeshAgent의 정지 거리 설정
    }

    public override void Execute()
    {
        if (boss.target == null)
        {
            boss.target = boss.FindTarget(); // 타겟을 찾는 메서드 호출
            if (boss.target == null)
            {
                boss.ChangeState(BossStates.Idle);
                return;
            }
        }
        // 거리가 먼 경우 추격 상태로 변경
        if (Vector3.Distance(boss.target.transform.position, boss.transform.position) > distanceToStand + adjustDistanceThreshold)
        {
            if (timer <= 0)
            {
                StartChase();
            }
        }
        if (state == AIBattleState.Idle)
        {
            if (timer <= 0)
            {
                StartChase();
            }
            else
            {
                // 공격 범위 내에서는 플레이어 주변을 맴도는 목적지 설정
                Vector3 directionToPlayer = (boss.target.transform.position - transform.position).normalized;
                Vector3 orbitPosition = boss.target.transform.position - directionToPlayer * (boss.stats.attackRange.GetValue() - 0.1f);
                boss.navAgent.SetDestination(orbitPosition);
                // 수동 회전 처리
                directionToPlayer.y = 0;

                if (directionToPlayer != Vector3.zero)
                {
                    float dot = Vector3.Dot(boss.transform.forward, directionToPlayer);

                    if (dot < 0.9f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
                    }
                }
            }
        }
        else if (state == AIBattleState.Chase)
        {
            if (Vector3.Distance(boss.target.transform.position, boss.transform.position) <= distanceToStand + adjustDistanceThreshold)
            {
                boss.ChangeState(BossStates.Attack);

                // 공격 후 대기 상태로 전환
                StartIdle();
                return;
            }
            boss.navAgent.SetDestination(boss.target.transform.position); // 타겟의 위치로 NavMeshAgent의 목적지 설정
        }
    }

    private void StartIdle()
    {
        state = AIBattleState.Idle;
        timer = boss.stats.attackInterval.GetValue(); // 공격 주기 타이머 설정
    }

    // 추격 상태 시작
    void StartChase()
    {
        state = AIBattleState.Chase;
    }

    public override void Exit()
    {
        boss.navAgent.ResetPath();
    }
}
