using UnityEngine;

public class PlayerSkillState : PlayerState
{
    public PlayerSkillState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator) { }

    public override void Enter()
    {
        animator.SetFloat("Speed", 0f);
        SkillData skillToUse = stateMachine.CurrentSkillToUse;
        animator.SetTrigger(skillToUse.animationTriggerName);

        switch (skillToUse.skillType)
        {
            case SkillType.Attack:
                break;

            case SkillType.Buff:
                stateMachine.Stats.ApplyBuff(skillToUse);
                break;
        }
    }

}
