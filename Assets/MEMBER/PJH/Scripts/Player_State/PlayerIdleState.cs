using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator) { }

    public override void Enter()
    {
        animator.SetFloat("Speed", 0);
        Debug.Log("Idle State 진입");
    }

    public override void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) // 이동상태 전환
        {
            stateMachine.ChangeState(new PlayerMoveState(stateMachine, player, animator));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) // 회피상태 전환
        {
            stateMachine.ChangeState(new PlayerEvasionState(stateMachine, player, animator));
        }

        //if (Input.GetKeyDown(KeyCode.Tab)) // 가드상태 전환
        //{
        //    stateMachine.ChangeState(new PlayerGuardState(stateMachine, player, animator));
        //}

        if (Input.GetKeyDown(KeyCode.LeftControl)) // 패링상태 전환
        {
            stateMachine.ChangeState(new PlayerParryingState(stateMachine, player, animator));
        }

        if (Input.GetMouseButtonDown(0))  // 공격상태 전환
        {
            stateMachine.ChangeState(new PlayerAttackState(stateMachine, player, animator));
        }


    }
    public override void Exit()
    {
        Debug.Log("Idle State 탈출");
    }
}
