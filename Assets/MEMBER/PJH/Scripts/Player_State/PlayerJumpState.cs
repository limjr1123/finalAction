using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private Rigidbody _rigidbody;
    private readonly float _jumpForce;
    private readonly float _groundCheckDistance;
    private readonly LayerMask _groundLayer;

    public PlayerJumpState(PlayerStateMachine stateMachine, GameObject player, Animator animator, Rigidbody rigidbody, float jumpForce, float groundCheckDistance, LayerMask groundLayer)
        : base(stateMachine, player, animator) 
    {
        _rigidbody = rigidbody;
        _jumpForce = jumpForce;
        _groundCheckDistance = groundCheckDistance;
        _groundLayer = groundLayer;
    }


    public override void Enter()
    {
        animator.SetBool("IsGrounded", false);
        animator.SetTrigger("Jump");
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        stateMachine.SoundSFX.PlayJumpSound();
    }

    public override void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    public override void Update()
    {
        if (_rigidbody.linearVelocity.y < 0f && IsGrounded())
        {
            animator.SetBool("IsGrounded", true);

            if (stateMachine.InputX == 0 && stateMachine.InputY == 0)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.MoveState);
            }
        }
    }

    private void Move(float fixedDeltaTime)
    {
        Vector3 newVelocity = new Vector3(
            stateMachine.MoveDirection.x * stateMachine.airControlSpeed,
            _rigidbody.linearVelocity.y,
            stateMachine.MoveDirection.z * stateMachine.airControlSpeed
        );

        _rigidbody.linearVelocity = newVelocity;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(player.transform.position, Vector3.down, _groundCheckDistance, _groundLayer);
    }
}


