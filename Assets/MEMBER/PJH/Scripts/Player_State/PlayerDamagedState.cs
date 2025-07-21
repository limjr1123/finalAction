using UnityEngine;

public class PlayerDamagedState : PlayerState
{
    public PlayerDamagedState(PlayerStateMachine stateMachine, GameObject player, Animator animator) 
        : base(stateMachine, player, animator) {}

    public override void Enter()
    {
        animator.SetTrigger("Damaged");

        stateMachine.MoveDirection = Vector3.zero;
        stateMachine.Rb.linearVelocity = Vector3.zero;

        //Debug.Log("플레이어 피격");
    }

}
