using UnityEngine;

public class PlayerGuardState : PlayerState
{
    public PlayerGuardState(PlayerStateMachine stateMachine, GameObject player, Animator animator) 
        : base(stateMachine, player, animator) {}

    public override void Enter()
    {
        animator.SetTrigger("Blocking");

        stateMachine.MoveDirection = Vector3.zero;
        stateMachine.Rb.linearVelocity = Vector3.zero;

        Debug.Log("°¡µå");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
