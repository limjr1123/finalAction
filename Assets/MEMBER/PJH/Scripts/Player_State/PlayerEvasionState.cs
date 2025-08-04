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

        SetLayerRecursively(player, _dodgingLayer);

        if (stateMachine.InputX != 0 || stateMachine.InputY != 0)
        {
            evasionDirection = stateMachine.MoveDirection;
        }
        else
        {
            evasionDirection = player.transform.forward;
        }

        stateMachine.Rb.AddForce(evasionDirection * evasionForce, ForceMode.Impulse);

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        SetLayerRecursively(player, _originalLayer);
   
        stateMachine.Rb.linearVelocity = Vector3.zero;
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}