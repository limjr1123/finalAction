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

        Debug.Log("Attack Combo: " + stateMachine.comboCount);

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
            stateMachine.ChangeState(stateMachine.AttackState);
        }
    }

    public override void AllowCombo()
    {
        isComboInputPossible = true;
    }
}