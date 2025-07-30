using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] Transform arrowHead; // 화살촉
    [SerializeField] Transform arrowTail; // 화살꼬리
    EnemyStat enemyStat; // 적의 스탯 컴포넌트
    int damage; // 화살 데미지

    BowController bow; // 활 컨트롤러

    private void Start()
    {
        enemyStat = GetComponentInParent<EnemyStat>();
        damage = enemyStat.attackDamageRange.CalculateDamage(enemyStat.criticalChance.GetValue(), enemyStat.criticalMultiplier.GetValue());

        bow = GetComponentInParent<BowController>();
    }

    private void Update()
    {
        Vector3 direction = (bow.bowAim.position - bow.arrowSpawnPoint.position).normalized;

        // Y축이 direction을 향하도록 회전 계산
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);
        transform.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // HitBox와 충돌했을 때 호출되는 메서드.
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>()?.TakePhysicalDamage(damage);
        }
    }
}
