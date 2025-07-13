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
            StartChase();
        }

        if (state == AIBattleState.Idle)
        {
            if (timer <= 0)
            {
                StartChase();
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