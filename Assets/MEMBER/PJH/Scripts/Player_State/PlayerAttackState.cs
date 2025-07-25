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
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
        {
            isComboInputPossible = true;
        }

        if (isComboInputPossible && Input.GetMouseButtonDown(0))
        {
            if (stateMachine.comboCount < 4)
            {
                stateMachine.ChangeState(new PlayerAttackState(stateMachine, player, animator));
            }
        }
    }
}