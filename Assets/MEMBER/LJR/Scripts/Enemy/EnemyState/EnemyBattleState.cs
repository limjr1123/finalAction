using System.Collections;
using UnityEngine;

public enum AIBattleState
{
    Idle,
    Chase,
    Attack
}

public class EnemyBattleState : EnemyState<EnemyController>
{
    EnemyController enemy;
    AIBattleState state;

    [SerializeField] float distanceToStand;        // 타겟과의 기본 유지 거리
    [SerializeField] float adjustDistanceThreshold = 0.1f;    // 거리 조정 허용 오차

    float timer = 0;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        distanceToStand = enemy.stats.attackRange.GetValue(); // 공격 범위에 따라 거리 설정
        enemy.navAgent.stoppingDistance = distanceToStand; // NavMeshAgent의 정지 거리 설정
    }

    public override void Execute()
    {
        if (enemy.target == null)
        {
            enemy.target = enemy.FindTarget(); // 타겟을 찾는 메서드 호출
            if (enemy.target == null)
            {
                enemy.ChangeState(EnemyStates.Idle);
                return;
            }
        }

        // 거리가 먼 경우 추격 상태로 변경
        if (Vector3.Distance(enemy.target.transform.position, enemy.transform.position) > distanceToStand + adjustDistanceThreshold)
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
                Vector3 directionToPlayer = (enemy.target.transform.position - transform.position).normalized;
                Vector3 orbitPosition = enemy.target.transform.position - directionToPlayer * (enemy.stats.attackRange.GetValue() - 0.1f);

                enemy.navAgent.SetDestination(orbitPosition);

                // 수동 회전 처리
                directionToPlayer.y = 0;

                if (directionToPlayer != Vector3.zero)
                {
                    float dot = Vector3.Dot(enemy.transform.forward, directionToPlayer);

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
            if (Vector3.Distance(enemy.target.transform.position, enemy.transform.position) <= distanceToStand + adjustDistanceThreshold)
            {
                enemy.ChangeState(EnemyStates.Attack);
                
                // 공격 후 대기 상태로 전환
                StartIdle();
                return;
            }
            enemy.navAgent.SetDestination(enemy.target.transform.position); // 타겟의 위치로 NavMeshAgent의 목적지 설정
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    // 대기 상태 시작
    private void StartIdle()
    {
        state = AIBattleState.Idle;
        timer = enemy.stats.attackInterval.GetValue(); // 공격 주기 타이머 설정
    }

    // 추격 상태 시작
    void StartChase()
    {
        state = AIBattleState.Chase;
    }

    public override void Exit()
    {
        enemy.navAgent.ResetPath();
    }
}