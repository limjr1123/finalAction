using UnityEngine;

public class PlayerGuardState : PlayerState
{
    private bool _isExiting = false;
    private float _timer;
    private readonly float _exitDuration;

    public PlayerGuardState(PlayerStateMachine stateMachine, GameObject player, Animator animator, float exitDuration) 
        : base(stateMachine, player, animator) 
    {
        _exitDuration = exitDuration;
    }

    public override void Enter()
    {
        animator.SetBool("IsBlocking", true);

        _isExiting = false;
    }

    public override void Update()
    {
        if (_isExiting)
        {
            _timer += Time.deltaTime;
            if (_timer >= _exitDuration)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
    }

    public override void FixedUpdate()
    {
        stateMachine.MoveDirection = Vector3.zero;
        stateMachine.Rb.linearVelocity = Vector3.zero;
    }

    public override void OnGuardUp()
    {
        if (!_isExiting)
        {
            animator.SetBool("IsBlocking", false);
            _isExiting = true;
            _timer = 0f;
        }
    }

    public override void OnGuardSuccess()
    {
        animator.SetTrigger("BlockImpact");
    }

    public override void Exit()
    {
        animator.SetBool("IsBlocking", false);
    }

    public override void OnAttack() { if (_isExiting) return; }
    public override void OnJump() { if (_isExiting) return; }
    public override void OnDodge() { if (_isExiting) return; }
}
