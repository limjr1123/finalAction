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
        if (Mathf.Abs(stateMachine.InputX) > 0.1f || Mathf.Abs(stateMachine.InputY) > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
    }

    public override void OnAttack()
    {
        Transform target = stateMachine.FindAutoTarget();
        if (target != null)
        {
            Vector3 targetDir = target.position - player.transform.position;
            float currentDistance = targetDir.magnitude;
            targetDir.y = 0;
            player.transform.rotation = Quaternion.LookRotation(targetDir);
            if (currentDistance > 1.6f)
                stateMachine.Rb.AddForce(player.transform.forward * stateMachine.dashForce, ForceMode.Impulse);
        }

        if (stateMachine.Stats.TryUseStamina(stateMachine.attackStaminaCost))
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
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
        Transform target = stateMachine.FindAutoTarget();
        if (target != null)
        {
            Vector3 targetDir = target.position - player.transform.position;
            float currentDistance = targetDir.magnitude;
            targetDir.y = 0;
            player.transform.rotation = Quaternion.LookRotation(targetDir);
        }

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
