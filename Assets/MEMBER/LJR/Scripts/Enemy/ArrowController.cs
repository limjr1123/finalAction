using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] Transform arrowHead;       // 화살촉
    [SerializeField] Transform arrowTail;       // 화살꼬리
    public BoxCollider hitBox;      // 화살의 HitBox
    public Rigidbody rb;            // 화살의 Rigidbody
    public float arrowSpeed = 50f;  // 화살 속도

    [SerializeField] public BowController bow; // 활 컨트롤러
    [SerializeField] int damage; // 화살 데미지
    [SerializeField] private float pullBackDistance = 1f; // 뒤로 당길 거리
    
    private HitEffectType hitEffectType = HitEffectType.Hit; // 히트 이펙트 타입

    public bool isShooting { get; set; } = false; // 화살이 발사 중인지 여부
    public bool isHit { get; set; } = false;      // 화살이 적에게 맞았는지 여부

    private void Start()
    {
        hitBox = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 화살의 방향을 활의 모양에 맞게 조정
        if (!isShooting && !isHit)
        {
            SetDirection(bow.bowDirection);
        }
    }

    public void SetDamage()
    {
        damage = bow.damage;
    }

    public void ResetArrow()
    {
        isShooting = false;
        isHit = false;
        rb.isKinematic = true;
    }

    public void ShootingArrow(Vector3 _direction)
    {
        SetDirection(_direction);   // 화살의 방향 설정(조준 방향으로 향하도록)
        isShooting = true;          // 화살이 발사 중임을 표시
        hitBox.enabled = true;      // collider 비활성화
        rb.isKinematic = false;     // Rigidbody를 활성화하여 물리 엔진에 의해 움직일 수 있도록 설정
        rb.linearVelocity = transform.up * arrowSpeed;
        transform.SetParent(null);
    }

    public void SetDirection(Vector3 _Direction)
    {
        Vector3 direction = _Direction.normalized;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);
        transform.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 pullBackDirection = -rb.linearVelocity.normalized;
        transform.position += pullBackDirection * pullBackDistance;

        if (other.CompareTag("PlayerGuard") || other.CompareTag("PlayerParry"))
        {
            Debug.Log("가드 성공.");
            transform.SetParent(other.transform);
            isHit = true;
            rb.isKinematic = true;
            hitBox.enabled = false;
            HitEffectManager.Instance.EffectCreate(transform, HitEffectType.Block);
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>()?.TakePhysicalDamage(damage);
            transform.SetParent(other.transform);
            isHit = true;
            rb.isKinematic = true;
            hitBox.enabled = false;
            HitEffectManager.Instance.EffectCreate(transform, hitEffectType);
        }
    }

    private void OnEnable()
    {
        ResetArrow();
    }
}
