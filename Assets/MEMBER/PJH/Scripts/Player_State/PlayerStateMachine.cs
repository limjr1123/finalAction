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
    public PlayerSkillState SkillState { get; private set; }

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
    public float groundCheckDistance = 1.1f;
    public LayerMask groundLayer;

    [Header("이동 관련")]
    public Vector3 MoveDirection { get; set; }
    public float InputX { get; private set; }
    public float InputY { get; private set; }
    public float rotationSpeed = 15f;
    public float airControlSpeed = 5f;
    public bool IsSprinting { get; set; }

    [Header("스킬 관련")]
    public SkillData[] skills = new SkillData[2];
    public SkillData CurrentSkillToUse { get; private set; }
    private float[] skillCooldowns;

    [Header("소모 관련")]
    public int dodgeStaminaCost = 10;
    public int AttackStaminaCost = 10;

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
        SkillState = new PlayerSkillState(this, gameObject, Animator);

        skillCooldowns = new float[skills.Length]; // 스킬 쿨타임 초기화
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

        for (int i = 0; i < skillCooldowns.Length; i++)  // 쿨타임 계산
        {
            if (skillCooldowns[i] > 0)
            {
                skillCooldowns[i] -= Time.deltaTime;
            }
        }
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

        if (Input.GetKeyDown(KeyCode.LeftControl)) // 점프
        {
            currentState?.OnJump(); 
        }

        if (Input.GetKeyDown(KeyCode.Space)) // 회피
        {   
            currentState?.OnDodge();
        }

        //if (Input.GetKeyDown(KeyCode.LeftControl)) // 패링
        //{
        //    currentState?.OnParry();
        //}

        //if (Input.GetKeyDown(KeyCode.Tab)) // 가드
        //{
        //    currentState?.OnGuard();
        //}

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentState?.OnGuard();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            currentState?.OnGuardUp();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentState?.OnSkill(0);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            currentState?.OnSkill(1);
        }
    }

    public bool TryUseSkill(int slotIndex)
    {
        // 슬롯 번호가 유효한지 확인
        if (slotIndex < 0 || slotIndex >= skills.Length || skills[slotIndex] == null) return false;

        // 해당 스킬의 쿨타임이 끝났는지 확인
        if (skillCooldowns[slotIndex] > 0)
        {
            Debug.Log($"{skills[slotIndex].skillName}의 쿨타임이 아직 끝나지 않았습니다.");
            return false;
        }

        // 해당 스킬의 마나 비용을 확인
        if (!Stats.TryUseMana(skills[slotIndex].manaCost))
        {
            Debug.Log("마나가 부족합니다.");
            return false;
        }

        // 모든 조건을 통과했으면 스킬 사용 준비
        CurrentSkillToUse = skills[slotIndex];
        skillCooldowns[slotIndex] = CurrentSkillToUse.cooldown;

        // 성공했다는 의미로 true를 반환
        return true;
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
        Animator.ResetTrigger("Attack");
        Animator.ResetTrigger("NextCombo");
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

    public void OnSkillAnimationEnd() // 스킬 애니메이션 종료 후 상태 전환
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
}


