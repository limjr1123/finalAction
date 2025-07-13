using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] PlayerController target;            // 공격 대상
    [SerializeField] SphereCollider sphereCollider;      // 범위 감지용 콜라이더
    [SerializeField] EnemyController enemyController;    // EnemyController에 타겟 설정용도

    private void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        if (enemyController == null)
        {
            Debug.LogError("EnemyController 컴포넌트가 없음.");
            return;
        }
        sphereCollider = GetComponent<SphereCollider>();

        if (sphereCollider == null)
        {
            Debug.LogError("EnemyVision collider 없음.");
            return;
        }
    }

    private void Update()
    {
        // 범위 감지용 콜라이더의 반지름을 적의 어그로 범위(aggroRange)로 설정
        sphereCollider.radius = enemyController.stats.aggroRange.GetValue();
        Debug.Log(sphereCollider.radius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
            return;
        target = other.GetComponent<PlayerController>();
        if (target != null)
            enemyController.target = target.gameObject;
    }
}
