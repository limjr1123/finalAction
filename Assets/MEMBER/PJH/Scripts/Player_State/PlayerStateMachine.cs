using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentState;
    public Animator animator;
    public PlayerStats Stats { get; private set; }
    public float rotationSpeed = 15f;

    [Header("���� ����")]
    public float comboTime = 1.5f;
    public int comboCount = 0;
    public float lastAttackTime = 0f;

    public Transform mainCamera;
    public Rigidbody Rb { get; private set; }
    public Animator Animator { get; private set; }
    public Vector3 MoveDirection { get; set; }
    public float InputX { get; private set; }
    public float InputY { get; private set; }

    void Start()
    {
        Rb = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        Stats = GetComponent<PlayerStats>();

        if (Stats != null)
        {
            PlayerStats.OnPlayerDied += Die;
        }


        ChangeState(new PlayerIdleState(this, gameObject, animator));
    }

    void Update()
    {
        if (currentState is PlayerAttackState && Time.time - lastAttackTime > comboTime)
        {
            ResetCombo();
        }

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
        ChangeState(new PlayerIdleState(this, gameObject, Animator));
    }

    public void GetDamage()  // �ǰ� ���� ��ȯ
    {
        if (currentState is PlayerDeathState || currentState is PlayerDamagedState)
        {
            return;
        }

        ChangeState(new PlayerDamagedState(this, gameObject, Animator));
    }

    public void OnHitAnimationEnd()  // �ǰ� �ִϸ��̼� ���� �� ���� ��ȯ
    {
        ChangeState(new PlayerIdleState(this, gameObject, Animator));
    }

    public void OnEvasionAnimationEnd() // ȸ�� �ִϸ��̼� ���� �� ���� ��ȯ
    {
        ChangeState(new PlayerIdleState(this, gameObject, Animator));
    }

    public void OnParryAnimationEnd() // �и� �ִϸ��̼� ���� �� ���� ��ȯ
    {
        ChangeState(new PlayerIdleState(this, gameObject, Animator));
    }

    public void Die()  // ��� ���� ��ȯ
    {
        if (currentState is PlayerDeathState) return; // �ߺ� ���� ����

        ChangeState(new PlayerDeathState(this, gameObject, Animator));
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
}


