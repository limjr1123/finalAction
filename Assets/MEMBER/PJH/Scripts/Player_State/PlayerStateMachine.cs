using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState currentState { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerEvasionState EvasionState { get; private set; }
    public PlayerDamagedState DamagedState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }
    public PlayerParryingState ParryingState { get; private set; }  
    public PlayerGuardState GuardState { get; private set; }

    [Header("필수 컴포넌트")]
    public PlayerStats Stats { get; private set; }
    public Transform mainCamera;
    public Rigidbody Rb { get; private set; }
    public Animator Animator { get; private set; }

    [Header("공격 관련")]
    public float comboTime = 1.5f;
    public int comboCount = 0;
    public float lastAttackTime = 0f;

    [Header("점프 관련")]
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    [Header("이동 관련")]
    public Vector3 MoveDirection { get; set; }
    public float InputX { get; private set; }
    public float InputY { get; private set; }
    public float rotationSpeed = 15f;
    public float airControlSpeed = 5f;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        Stats = GetComponent<PlayerStats>();

        IdleState = new PlayerIdleState(this, gameObject, Animator);
        MoveState = new PlayerMoveState(this, gameObject, Animator);
        JumpState = new PlayerJumpState(this, gameObject, Animator, Rb, jumpForce, groundCheckDistance, groundLayer);
        AttackState = new PlayerAttackState(this, gameObject, Animator);
        EvasionState = new PlayerEvasionState(this, gameObject, Animator);
        DamagedState = new PlayerDamagedState(this, gameObject, Animator);
        DeathState = new PlayerDeathState(this, gameObject, Animator);
        ParryingState = new PlayerParryingState(this, gameObject, Animator);
        GuardState = new PlayerGuardState(this, gameObject, Animator);
    }

    void Start()
    {
        if (Stats != null)
        {
            PlayerStats.OnPlayerDied += Die;
        }

        ChangeState(IdleState);
    }

    void Update()
    {
        if (currentState is PlayerAttackState && Time.time - lastAttackTime > comboTime)
        {
            ResetCombo();
        }
        CalculateMoveDirection();
        HandleInput();
        currentState?.Update();
    }

    void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    void HandleInput()
    {
        InputX = Input.GetAxisRaw("Horizontal");
        InputY = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonDown(0))  // 공격
        {
            currentState?.OnAttack();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)) // 점프
        {
            currentState?.OnJump(); 
        }

        if (Input.GetKeyDown(KeyCode.Space)) // 회피
        {
            currentState?.OnDodge();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            currentState?.OnParry();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentState?.OnGuard();
        }
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    public void ResetCombo()
    {
        comboCount = 0;
        Animator.SetInteger("ComboCount", 0);
        ChangeState(IdleState);
    }

    public void GetDamage()  // 피격 상태 전환
    {
        if (currentState is PlayerDeathState || currentState is PlayerDamagedState)
        {
            return;
        }

        ChangeState(DamagedState);
    }

    public void OnHitAnimationEnd()  // 피격 애니메이션 종료 후 상태 전환
    {
        ChangeState(IdleState);
    }

    public void OnEvasionAnimationEnd() // 회피 애니메이션 종료 후 상태 전환
    {
        ChangeState(IdleState);
    }

    public void OnParryAnimationEnd() // 패링 애니메이션 종료 후 상태 전환
    {
        ChangeState(IdleState);
    }
    public void OnGuaurdAnimationEnd() // 패링 애니메이션 종료 후 상태 전환
    {
        ChangeState(IdleState);
    }

    public void Die()  // 사망 상태 전환
    {
        if (currentState is PlayerDeathState) return; // 중복 실행 방지

        ChangeState(DeathState);
    }

    void OnDisable()
    {
        if (Stats != null)
        {
            PlayerStats.OnPlayerDied -= Die;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            GetDamage();
        }
    }

    private void CalculateMoveDirection()
    {
        Vector3 camForward = mainCamera.forward;
        Vector3 camRight = mainCamera.right;
        camForward.y = 0;
        camRight.y = 0;

        MoveDirection = (camForward.normalized * InputY + camRight.normalized * InputX).normalized;
    }

    public void AnimationEvent_AllowCombo()
    {
        currentState?.AllowCombo();
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}


