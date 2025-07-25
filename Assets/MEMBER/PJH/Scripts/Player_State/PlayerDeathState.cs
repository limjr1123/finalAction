using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator) { }

    public override void Enter()
    {
        base.Enter();
        animator.SetTrigger("Die");

        // ������x
        stateMachine.Rb.linearVelocity = Vector3.zero;
        stateMachine.MoveDirection = Vector3.zero; 

        Debug.Log("�÷��̾� ��� ����");
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
