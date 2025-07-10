using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�ӵ� ����")]
    public float RunningSpeed = 5f;
    public float SprintSpeed = 10f;

    public Transform Cam; // ī�޶� Transform

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

        if (!stateInfo.IsTag("Attack")) //�ִϸ����� ���°� Attack�̸� �̵�x
        {
            X = Input.GetAxisRaw("Horizontal");
            Y = Input.GetAxisRaw("Vertical");

            // ī�޶� ���� ����
            Vector3 camForward = Cam.forward;
            Vector3 camRight = Cam.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // �Է� ������ ī�޶� �������� ��ȯ
            Vector3 moveDir = camForward * Y + camRight * X;
            moveDir.Normalize();


            Player_anim.SetFloat("Speed", moveDir.magnitude);
            Player_anim.SetFloat("MoveX", moveDir.x);
            Player_anim.SetFloat("MoveY", moveDir.z);

            Sprint = Input.GetKey(KeyCode.LeftShift);  // ���� ����Ʈ = ������Ʈ
            Player_anim.SetBool("IsSprint", Sprint);

            float speed = Sprint ? SprintSpeed : RunningSpeed;
            Moving = moveDir * speed;

            if (Input.GetKeyDown(KeyCode.Space))
                Player_anim.SetTrigger("Evasion"); // �����̽��� = ȸ��
        }
    }
    void FixedUpdate()
    {
        Player_rb.MovePosition(Player_rb.position + Moving * Time.fixedDeltaTime);
    }
}
