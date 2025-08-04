using UnityEngine;

public class PlayerGuardState : PlayerState
{

    public PlayerGuardState(PlayerStateMachine stateMachine, GameObject player, Animator animator) 
        : base(stateMachine, player, animator) {}

    public override void Enter()
    {
        animator.SetBool("IsBlocking", true);
    
    }

    public override void FixedUpdate()
    {
        stateMachine.MoveDirection = Vector3.zero;
        stateMachine.Rb.linearVelocity = Vector3.zero;
    }

    public override void OnGuardUp()
    {
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    public override void Exit()
    {
        animator.SetBool("IsBlocking", false);
    }
}
