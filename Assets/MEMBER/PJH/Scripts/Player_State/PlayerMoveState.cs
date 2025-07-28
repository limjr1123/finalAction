using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator)
    { }

    public override void Enter()
    {
    }

    public override void Update()
    {
        if (stateMachine.InputX == 0 && stateMachine.InputY == 0)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine, player, animator));
            return;
        }

   
        Rotate(Time.deltaTime);
        UpdateAnimator(Time.deltaTime);
    }

    public override void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    public override void OnAttack()
    {
        stateMachine.ChangeState(stateMachine.AttackState);
    }
    public override void OnJump()
    {
        stateMachine.ChangeState(stateMachine.JumpState);
    }

    public override void OnDodge()
    {
        stateMachine.ChangeState(stateMachine.EvasionState);
    }

    public override void OnParry()
    {
        stateMachine.ChangeState(stateMachine.ParryingState);
    }

    public override void OnGuard()
    {
        stateMachine.ChangeState(stateMachine.GuardState);
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

    private void UpdateAnimator(float deltaTime)
    {
        float moveAmount = Mathf.Abs(stateMachine.InputX) + Mathf.Abs(stateMachine.InputY);
        moveAmount = Mathf.Clamp01(moveAmount);

        animator.SetFloat("Speed", moveAmount, 0.1f, deltaTime);

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("IsSprint", isSprinting);
    }
}
