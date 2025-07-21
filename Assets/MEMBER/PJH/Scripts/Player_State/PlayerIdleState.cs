using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator) { }

    public override void Enter()
    {
        animator.SetFloat("Speed", 0);
        Debug.Log("Idle State ����");
    }

    public override void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) // �̵����� ��ȯ
        {
            stateMachine.ChangeState(new PlayerMoveState(stateMachine, player, animator));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) // ȸ�ǻ��� ��ȯ
        {
            stateMachine.ChangeState(new PlayerEvasionState(stateMachine, player, animator));
        }

        //if (Input.GetKeyDown(KeyCode.Tab)) // ������� ��ȯ
        //{
        //    stateMachine.ChangeState(new PlayerGuardState(stateMachine, player, animator));
        //}

        if (Input.GetKeyDown(KeyCode.LeftControl)) // �и����� ��ȯ
        {
            stateMachine.ChangeState(new PlayerParryingState(stateMachine, player, animator));
        }

        if (Input.GetMouseButtonDown(0))  // ���ݻ��� ��ȯ
        {
            stateMachine.ChangeState(new PlayerAttackState(stateMachine, player, animator));
        }


    }
    public override void Exit()
    {
        Debug.Log("Idle State Ż��");
    }
}
