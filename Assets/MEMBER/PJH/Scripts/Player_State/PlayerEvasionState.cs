using UnityEngine;

public class PlayerEvasionState : PlayerState
{
    private float evasionForce = 8f;
    private Vector3 evasionDirection;

    private readonly int _originalLayer;
    private readonly int _dodgingLayer;

    public PlayerEvasionState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
        : base(stateMachine, player, animator)
    {
        _originalLayer = player.layer;
        _dodgingLayer = LayerMask.NameToLayer("PlayerDodge");
    }

    public override void Enter()
    {
        animator.SetTrigger("Evasion");

        player.layer = _dodgingLayer;

        if (stateMachine.InputX != 0 || stateMachine.InputY != 0)
        {
            evasionDirection = stateMachine.MoveDirection;
        }
        else
        {
            evasionDirection = player.transform.forward;
        }

        if (evasionDirection != Vector3.zero)
        {
            player.transform.rotation = Quaternion.LookRotation(evasionDirection);
        }

        stateMachine.Rb.AddForce(evasionDirection * evasionForce, ForceMode.Impulse);

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        player.layer = _originalLayer;

        stateMachine.Rb.linearVelocity = Vector3.zero;
    }

}