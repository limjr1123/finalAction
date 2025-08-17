using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerStateMachine : MonoBehaviour
{
    public static PlayerStateMachine Instance { get; private set; }

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
    public PlayerSoundSFX SoundSFX { get; private set; }

    [Header("공격 관련")]
    public float comboTime = 1f;
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
    public int attackStaminaCost = 5;
    public int sprintStaminaCost = 2; 

    [Header("레이어 변경 관련")]
    private int _playerLayer;
    private int _evasionLayer;

    [Header("오토타겟팅 관련")]
    public float autoTargetingDistance = 10f; // 탐색 거리
    public float autoTargetingAngle = 60f;    // 탐색 각도
    public LayerMask enemyLayerMask;

    [Header("가드 관련")] 
    public float guardExitDuration = 0.6f;
    
    public Collider interactableCollider; // 상호작용할 수 있는 오브젝트의 콜라이더
    public LayerMask interactableLayerMask; // 상호작용 가능한 레이어

    private FixedJoystick joyStick;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // 이미 다른 PlayerStateMachine이 존재하면, 새로운 자신을 파괴합니다.
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Rb = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        Stats = GetComponent<PlayerStats>();
        SoundSFX = GetComponent<PlayerSoundSFX>();

        IdleState = new PlayerIdleState(this, gameObject, Animator);
        MoveState = new PlayerMoveState(this, gameObject, Animator);
        JumpState = new PlayerJumpState(this, gameObject, Animator, Rb, jumpForce, groundCheckDistance, groundLayer);
        AttackState = new PlayerAttackState(this, gameObject, Animator);
        EvasionState = new PlayerEvasionState(this, gameObject, Animator);
        DamagedState = new PlayerDamagedState(this, gameObject, Animator);
        DeathState = new PlayerDeathState(this, gameObject, Animator);
        ParryingState = new PlayerParryingState(this, gameObject, Animator);
        GuardState = new PlayerGuardState(this, gameObject, Animator, guardExitDuration);
        SkillState = new PlayerSkillState(this, gameObject, Animator);

        skillCooldowns = new float[skills.Length]; // 스킬 쿨타임 초기화

        _playerLayer = LayerMask.NameToLayer("Player"); 
        _evasionLayer = LayerMask.NameToLayer("PlayerDodge");
    }

    void Start()
    {
        if (Stats != null)
        {
            PlayerStats.OnPlayerDied += Die;
        }
        joyStick = FindFirstObjectByType<FixedJoystick>();

        ChangeState(IdleState);
    }

    void Update()
    {
        if (currentState is PlayerAttackState && Time.time - lastAttackTime > comboTime)
        {
            ResetCombo();
        }

        if (joyStick != null)
        {
            InputX = joyStick.Horizontal;
            InputY = joyStick.Vertical;
        }

        CalculateMoveDirection();
        //HandleInput();
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

        float keyboardX = Input.GetAxisRaw("Horizontal");
        float keyboardY = Input.GetAxisRaw("Vertical");

        if (InputX != 0 || InputY != 0)
        {
            InputX = keyboardX;
            InputY = keyboardY;
        }

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

        if (Input.GetKeyDown(KeyCode.Tab))  // 가드
        {
            currentState?.OnGuard();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            currentState?.OnGuardUp();
        }

        if (Input.GetKeyDown(KeyCode.Q))  // 스킬1
        {
            currentState?.OnSkill(0);
        }
        else if (Input.GetKeyDown(KeyCode.E))  // 스킬2
        {
            currentState?.OnSkill(1);
        }

        if(Input.GetKeyDown(KeyCode.G)) // 상호작용
        {
            Interact();
        }
    }

    public void Interact()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, interactableLayerMask))
        {
            hit.collider.GetComponent<NPC>()?.Interact();
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
        if (currentState is PlayerDeathState ||   //죽었거나
            currentState is PlayerDamagedState || // 피격 중이거나
            currentState is PlayerGuardState ||  // 가드 중이거나
            currentState is PlayerSkillState)  // 스킬 사용 중이면 경직x
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

    public void OnGuaurdAnimationEnd() // 가드 애니메이션 종료 후 상태 전환
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

    public void AnimationEvent_ChangeLayerToEvasion()
    {
        this.gameObject.layer = _evasionLayer;
    }

    public void AnimationEvent_RevertLayer()
    {
        this.gameObject.layer = _playerLayer;
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

    public void OnGuardSuccess()
    {                                             
        currentState?.OnGuardSuccess();
    }

    public void OnParrySuccess()
    {
        Animator.SetTrigger("Parrying");
    }

    public Transform FindAutoTarget()
    {
        // 1. 지정된 거리 내의 모든 적을 찾습니다.
        Collider[] colliders = Physics.OverlapSphere(transform.position, autoTargetingDistance, enemyLayerMask);

        Transform bestTarget = null;
        float minAngle = float.MaxValue;

        if (colliders.Length == 0) return null;

        // 2. 찾은 적들 중에서 가장 적합한 타겟을 고릅니다.
        foreach (var collider in colliders)
        {
            Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;
            directionToTarget.y = 0; // y축은 무시하여 수평 각도만 계산

            // 플레이어의 정면 방향과 타겟 방향 사이의 각도를 계산
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            // 3. 설정된 탐색 각도 안에 있고, 그 중 가장 정면에 있는 타겟을 선택
            if (angle < autoTargetingAngle / 2f)
            {
                if (angle < minAngle)
                {
                    minAngle = angle;
                    bestTarget = collider.transform;
                }
            }
        }
        return bestTarget;
    }

    public void AnimationEvent_PlayLeftFootstepSound()
    {
        SoundSFX?.PlayLeftFootstepSound();
    }

    public void AnimationEvent_PlayRightFootstepSound()
    {
        SoundSFX?.PlayRightFootstepSound();
    }

    public void AnimationEvent_PlayAttackSKillSound()
    {
        SoundSFX?.PlayAttackSkillSound();
    }

    public void AnimationEvent_PlayBuffSkillSound()
    {
        SoundSFX?.PlayBuffSkillSound();
    }
}


