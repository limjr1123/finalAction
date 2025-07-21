using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator)
    { }

    public override void Enter()
    {
        Debug.Log("Move State 진입");
    }

    public override void Update()
    {
        // 이동 입력이 없으면 Idle 상태로 전환
        if (stateMachine.InputX == 0 && stateMachine.InputY == 0)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine, player, animator));
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

        // 이동 방향 계산 및 캐릭터 회전
        CalculateMoveDirection();
        Rotate(Time.deltaTime);

        // 애니메이터 파라미터 설정
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
