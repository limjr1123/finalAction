using UnityEngine;

public class PlayerEvasionState : PlayerState
{
    private float evasionForce = 8f;
    private Vector3 evasionDirection;

    public PlayerEvasionState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator) { }

    public override void Enter()
    {
        animator.SetTrigger("Evasion");

        if (stateMachine.InputX != 0 || stateMachine.InputY != 0)
        {
            evasionDirection = stateMachine.MoveDirection;
        }
        else
        {
            evasionDirection = player.transform.forward;
        }

        stateMachine.Rb.AddForce(evasionDirection * evasionForce, ForceMode.Impulse);
        // ���¹̳��� �Ҹ� ���� �߰�

        Debug.Log("�÷��̾� ȸ��");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        stateMachine.Rb.linearVelocity = Vector3.zero;
    }
}