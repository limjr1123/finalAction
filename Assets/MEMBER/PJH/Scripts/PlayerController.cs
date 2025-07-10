using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("속도 관련")]
    public float RunningSpeed = 5f;
    public float SprintSpeed = 10f;

    public Transform Cam; // 카메라 Transform

    private Rigidbody Player_rb;
    private Animator Player_anim;
    private Vector3 Moving;

    private float X;
    private float Y;
    private bool Sprint;

    void Start()
    {
        Player_rb = GetComponent<Rigidbody>();
        Player_anim = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = Player_anim.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsTag("Attack")) //애니메이터 상태가 Attack이면 이동x
        {
            X = Input.GetAxisRaw("Horizontal");
            Y = Input.GetAxisRaw("Vertical");

            // 카메라 기준 방향
            Vector3 camForward = Cam.forward;
            Vector3 camRight = Cam.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // 입력 방향을 카메라 기준으로 변환
            Vector3 moveDir = camForward * Y + camRight * X;
            moveDir.Normalize();


            Player_anim.SetFloat("Speed", moveDir.magnitude);
            Player_anim.SetFloat("MoveX", moveDir.x);
            Player_anim.SetFloat("MoveY", moveDir.z);

            Sprint = Input.GetKey(KeyCode.LeftShift);  // 왼쪽 시프트 = 스프린트
            Player_anim.SetBool("IsSprint", Sprint);

            float speed = Sprint ? SprintSpeed : RunningSpeed;
            Moving = moveDir * speed;

            if (Input.GetKeyDown(KeyCode.Space))
                Player_anim.SetTrigger("Evasion"); // 스페이스바 = 회피
        }
    }
    void FixedUpdate()
    {
        Player_rb.MovePosition(Player_rb.position + Moving * Time.fixedDeltaTime);
    }
}
