using UnityEngine;

public class PlayerAttackState : PlayerState
{
    public PlayerAttackState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator) { }

    private bool isComboInputPossible = false;

    public override void Enter()
    {
        animator.SetFloat("Speed", 0f);
        stateMachine.lastAttackTime = Time.time;
        stateMachine.comboCount = Mathf.Clamp(stateMachine.comboCount + 1, 1, 4);
        stateMachine.SoundSFX.PlayAttackSound(stateMachine.comboCount);

        if (stateMachine.comboCount == 1)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            animator.SetTrigger("NextCombo");
        }

        isComboInputPossible = false;
    }

    public override void Update()
    {
     
    }

    public override void Exit()
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("NextCombo");
    }

    public override void OnAttack()
    {
        if (isComboInputPossible && stateMachine.comboCount < 4)
        {
            if (stateMachine.Stats.TryUseStamina(stateMachine.attackStaminaCost))
            {
                stateMachine.ChangeState(stateMachine.AttackState);
            }
            else
            {
                Debug.Log("스태미나 부족");
            }
        }
    }

    public override void AllowCombo()
    {
        isComboInputPossible = true;
    }
}