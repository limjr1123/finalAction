using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator) { }

    public override void Enter()
    {
        animator.SetFloat("Speed", 0);
    }

    public override void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) // 이동상태 전환
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
    }

    public override void OnAttack()
    {
        stateMachine.ChangeState(stateMachine.AttackState);
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

    public override void OnJump()
    {
        stateMachine.ChangeState(stateMachine.JumpState);
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


    public override void Exit()
    {
    }
}
