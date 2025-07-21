using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("플레이어 설정")]
    public float runningSpeed = 5f;
    public float sprintSpeed = 10f;
    public float rotationSpeed = 15f; // 캐릭터 회전 속도

    [Header("필수 컴포넌트")]
    public Transform mainCamera;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveDirection;
    private float inputX;
    private float inputY;

    private PlayerHealth playerHealth;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        playerHealth = GetComponent<PlayerHealth>();
        PlayerHealth.OnPlayerDied += OnDeath;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= OnDeath;
    }

    void OnDeath()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector3.zero;
        moveDirection = Vector3.zero;
    }

    void Update()
    {
        if (isDead) return;

        HandleInput();
        HandleAnimatorState();
        HandleActions();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        HandleMovement();
    }

    void HandleInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
    }

    void HandleAnimatorState()
    {
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
        bool isBlocking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Block");
        bool isParrying = animator.GetCurrentAnimatorStateInfo(0).IsTag("Parry");

        if (isAttacking || isBlocking || isParrying)
        {
            moveDirection = Vector3.zero; // 공격, 막기, 패링 중에는 이동x
        }
        else
        {
            // 카메라 기준 이동 방향 계산
            Vector3 camForward = mainCamera.forward;
            Vector3 camRight = mainCamera.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            moveDirection = (camForward * inputY + camRight * inputX).normalized;

            // 캐릭터 회전
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        animator.SetFloat("MoveX", inputX);
        animator.SetFloat("MoveY", inputY);
        animator.SetFloat("Speed", new Vector2(inputX, inputY).magnitude);
    }

    void HandleActions()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("IsSprint", isSprinting);

        if (Input.GetKeyDown(KeyCode.Space))
            animator.SetTrigger("Evasion");

        if (Input.GetKeyDown(KeyCode.Tab))
            animator.SetTrigger("Blocking");

        if (Input.GetKeyDown(KeyCode.LeftControl))
            animator.SetTrigger("Parrying");

    }

    void HandleMovement()
    {
        bool isSprinting = animator.GetBool("IsSprint");
        float currentSpeed = isSprinting ? sprintSpeed : runningSpeed;

        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }
}