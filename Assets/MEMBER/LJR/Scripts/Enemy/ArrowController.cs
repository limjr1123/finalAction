using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class ArrowController : MonoBehaviour
{
    [SerializeField] Transform arrowHead; // 화살촉
    [SerializeField] Transform arrowTail; // 화살꼬리
    [SerializeField] float arrowSpeed = 8f; // 화살 몸통
    BoxCollider hitBox;     // 화살의 HitBox
    Rigidbody rb;           // 화살의 Rigidbody

    BowController bow; // 활 컨트롤러
    int damage; // 화살 데미지

    public bool isShooting { get; set; } = false; // 화살이 발사 중인지 여부
    public bool isHit { get; set; } = false;      // 화살이 적에게 맞았는지 여부

    private void Start()
    {
        hitBox = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        bow = GetComponentInParent<BowController>();
    }

    private void Update()
    {
        if (!isShooting && !isHit)
        {
            SetDirection(bow.bowAim.position - bow.arrowSpawnPoint.position);
        }
        else if (isShooting && !isHit)
        {
            SetDirection(bow.arrowToTargetDirection);
            // 화살이 발사 중이고 적에게 맞지 않았다면 화살을 앞으로 이동
            transform.position += bow.arrowToTargetDirection * Time.deltaTime * arrowSpeed; // 속도는 10f로 설정
        }
    }

    private void SetDirection(Vector3 _Direction)
    {
        Vector3 direction = _Direction.normalized;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);
        transform.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>()?.TakePhysicalDamage(damage);
        }
        transform.SetParent(other.transform);

        isHit = true;
        rb.isKinematic = true;
        hitBox.enabled = false;
    }
}
