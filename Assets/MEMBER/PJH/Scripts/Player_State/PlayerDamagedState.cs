using UnityEngine;

public class PlayerDamagedState : PlayerState
{
    public PlayerDamagedState(PlayerStateMachine stateMachine, GameObject player, Animator animator) 
        : base(stateMachine, player, animator) {}

    public override void Enter()
    {
        animator.SetTrigger("Damaged");
        stateMachine.SoundSFX.PlayDamagedSound();
        stateMachine.MoveDirection = Vector3.zero;
        stateMachine.Rb.linearVelocity = Vector3.zero;
    }

    public override void Exit()
    {
        animator.ResetTrigger("Damaged");
        stateMachine.comboCount = 0; // 데미지를 입으면 콤보 초기화
    }

    }
