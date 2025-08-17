using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator)
    { }

    private float _staminaDrainAccumulator = 0f;

    public bool isSprinting = false;

    public override void Enter()
    {
    }

    public override void Update()
    {
        if (stateMachine.InputX == 0 && stateMachine.InputY == 0)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

   
        Rotate(Time.deltaTime);
        UpdateAnimator(Time.deltaTime);
    }

    public override void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    public override void Exit()
    {
        stateMachine.IsSprinting = false;
        animator.SetBool("IsSprint", false);
    }

    public override void OnAttack()
    {
        Transform target = stateMachine.FindAutoTarget();
        if (target != null)
        {
            Vector3 targetDir = target.position - player.transform.position;
            targetDir.y = 0;
            player.transform.rotation = Quaternion.LookRotation(targetDir);
        }

        if (stateMachine.Stats.TryUseStamina(stateMachine.attackStaminaCost))
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
    }

    public override void OnJump()
    {
        stateMachine.ChangeState(stateMachine.JumpState);
    }

    public override void OnDodge()
    {
        if (stateMachine.Stats.TryUseStamina(stateMachine.dodgeStaminaCost))
        {
            stateMachine.ChangeState(stateMachine.EvasionState);
        }
    }

    public override void OnParry()
    {
        stateMachine.ChangeState(stateMachine.ParryingState);
    }

    public override void OnGuard()
    {
        stateMachine.ChangeState(stateMachine.GuardState);
    }

    public override void OnSkill(int slotIndex)
    {
        if (stateMachine.TryUseSkill(slotIndex))
        {
            stateMachine.ChangeState(stateMachine.SkillState);
        }
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
        float moveAmount = Mathf.Clamp01(Mathf.Abs(stateMachine.InputX) + Mathf.Abs(stateMachine.InputY));
        float currentSpeed;
        //bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isSprinting = moveAmount > 0.6f;

        if (isSprinting)
        {
            _staminaDrainAccumulator += stateMachine.sprintStaminaCost * fixedDeltaTime;

            if (_staminaDrainAccumulator >= 1f)
            {
                int staminaToDrain = Mathf.FloorToInt(_staminaDrainAccumulator);
                if (stateMachine.Stats.TryUseStamina(staminaToDrain))
                {
                    _staminaDrainAccumulator -= staminaToDrain;
                    currentSpeed = stateMachine.Stats.sprintSpeed.GetValue();
                }
                else
                {
                    isSprinting = false;
                    currentSpeed = stateMachine.Stats.moveSpeed.GetValue();
                }
            }
            else
            {
                currentSpeed = stateMachine.Stats.sprintSpeed.GetValue();
            }
        }
        else
        {
            _staminaDrainAccumulator = 0f;
            currentSpeed = stateMachine.Stats.moveSpeed.GetValue();
        }

        stateMachine.IsSprinting = isSprinting;
        stateMachine.Rb.MovePosition(stateMachine.Rb.position + stateMachine.MoveDirection * currentSpeed * fixedDeltaTime);
    }

    private void UpdateAnimator(float deltaTime)
    {
        float moveAmount = Mathf.Abs(stateMachine.InputX) + Mathf.Abs(stateMachine.InputY);
        moveAmount = Mathf.Clamp01(moveAmount);

        animator.SetFloat("Speed", moveAmount, 0.1f, deltaTime);

        //bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isSprinting = moveAmount > 0.6f;
        animator.SetBool("IsSprint", isSprinting);
    }
}
