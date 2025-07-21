using UnityEngine;

public class PlayerParryingState : PlayerState
{
    public PlayerParryingState(PlayerStateMachine stateMachine, GameObject player, Animator animator) 
        : base(stateMachine, player, animator) { }

    public override void Enter()
    {
        animator.SetTrigger("Parrying");

        stateMachine.MoveDirection = Vector3.zero;
        stateMachine.Rb.linearVelocity = Vector3.zero;

        Debug.Log("ÆÐ¸µ");
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
