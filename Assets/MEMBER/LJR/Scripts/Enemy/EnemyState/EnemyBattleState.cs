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

    [SerializeField] float distanceToStand = 3f;        // 타겟과의 기본 유지 거리
    [SerializeField] float adjustDistanceThreshold = 1f;    // 거리 조정 허용 오차

    [SerializeField] Vector2 idleTimeRange = new Vector2(2, 3);     // 대기 상태 지속 시간 범위(초)
    float timer = 0;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.navAgent.stoppingDistance = distanceToStand; // NavMeshAgent의 정지 거리 설정
        enemy.battleMovementTimer = 0f;

        enemy.anim.SetBool("Battle", true);

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

        if (Vector3.Distance(enemy.target.transform.position, enemy.transform.position) > distanceToStand + adjustDistanceThreshold)
        {
            StartChase();
        }

        if (state == AIBattleState.Idle)
        {
            if (timer <= 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    StartIdle();
                }
            }
        }
        else if (state == AIBattleState.Chase)
        {
            if (Vector3.Distance(enemy.target.transform.position, enemy.transform.position) <= distanceToStand + 0.03f)
            {
                StartIdle();
                return;
            }
            enemy.navAgent.SetDestination(enemy.target.transform.position); // 타겟의 위치로 NavMeshAgent의 목적지 설정
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        enemy.battleMovementTimer += Time.deltaTime;

    }
    private void StartIdle()
    {
        state = AIBattleState.Idle;
        timer = Random.Range(idleTimeRange.x, idleTimeRange.y);
    }

    void StartChase()
    {
        state = AIBattleState.Chase;
    }

    public override void Exit()
    {
        enemy.navAgent.ResetPath();
    }
}