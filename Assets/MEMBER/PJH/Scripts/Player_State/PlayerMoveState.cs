using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator)
    { }

    public override void Enter()
    {
        Debug.Log("Move State ����");
    }

    public override void Update()
    {
        // �̵� �Է��� ������ Idle ���·� ��ȯ
        if (stateMachine.InputX == 0 && stateMachine.InputY == 0)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine, player, animator));
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

        // �̵� ���� ��� �� ĳ���� ȸ��
        CalculateMoveDirection();
        Rotate(Time.deltaTime);

        // �ִϸ����� �Ķ���� ����
        float moveAmount = new Vector2(stateMachine.InputX, stateMachine.InputY).magnitude;
        animator.SetFloat("Speed", moveAmount, 0.1f, Time.deltaTime);
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("IsSprint", isSprinting);
    }

    public override void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    private void CalculateMoveDirection()
    {
        Transform mainCamera = stateMachine.mainCamera;
        Vector3 camForward = mainCamera.forward;
        Vector3 camRight = mainCamera.right;
        camForward.y = 0;
        camRight.y = 0;

        stateMachine.MoveDirection = (camForward.normalized * stateMachine.InputY + camRight.normalized * stateMachine.InputX).normalized;
    }

    private void Rotate(float deltaTime)
    {
        if (stateMachine.MoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(stateMachine.MoveDirection);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, stateMachine.rotationSpeed * deltaTime);
        }
    }

    private void Move(float fixedDeltaTime)
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? stateMachine.Stats.sprintSpeed.GetValue() : stateMachine.Stats.moveSpeed.GetValue();
        stateMachine.Rb.MovePosition(stateMachine.Rb.position + stateMachine.MoveDirection * currentSpeed * fixedDeltaTime);
    }
}
