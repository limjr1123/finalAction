using UnityEngine;

public abstract class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected GameObject player;
    protected Animator animator;

    public PlayerState(PlayerStateMachine stateMachine, GameObject player, Animator animator)
    {
        this.stateMachine = stateMachine;
        this.player = player;
        this.animator = animator;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    public virtual void OnAttack() { }
    public virtual void OnJump() { }
    public virtual void OnDodge() { }
    public virtual void OnParry() { }
    public virtual void OnGuard() { }
    public virtual void OnGuardUp() { }
    public virtual void OnGuardSuccess() { }
    public virtual void AllowCombo() { }
    public virtual void OnSkill(int slotIndex) { }
}
