using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("속도 관련")]
    public float RunningSpeed = 5f;
    public float SprintSpeed = 10f;

    private Animator Player_anim;
    private Rigidbody Player_rb;
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
        X = Input.GetAxisRaw("Horizontal");
        Y = Input.GetAxisRaw("Vertical");

        Vector3 InputDir = new Vector3(X, 0f, Y).normalized;

        Player_anim.SetFloat("Speed", InputDir.magnitude);

        Player_anim.SetFloat("MoveX", InputDir.x);
        Player_anim.SetFloat("MoveY", InputDir.z);

        Sprint = Input.GetKey(KeyCode.LeftShift); // 왼쪽 시프트 = 스프린트
        Player_anim.SetBool("IsSprint", Sprint);

        float Speed = Sprint ? SprintSpeed : RunningSpeed;
        Moving = InputDir * Speed;

    }


    void FixedUpdate()
    {
        Player_rb.MovePosition(Player_rb.position + Moving * Time.fixedDeltaTime);
    }
}