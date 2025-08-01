using System.Collections;
using UnityEngine;

public class BowController : MonoBehaviour
{
    public Animator anim { get; set; }  // 무기 애니메이터
    public Transform bowAim;            // 활의 조준 위치
    public Transform arrowSpawnPoint;   // 화살이 생성될 위치
    public GameObject arrowPrefab;      // 생성할 화살 프리팹
    public GameObject arrow;            // 생성된 화살 오브젝트
    public Vector3 arrowToTargetDirection;  // 조준 방향 벡터

    EnemyController enemyController;    // 적 컨트롤러
    EnemyStat enemyStat;                // 적의 스탯 컴포넌트

    int damage; // 화살 데미지

    void Awake()
    {
        anim = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        enemyController = GetComponentInParent<EnemyController>();
        enemyStat = GetComponentInParent<EnemyStat>();
    }

    private void Start()
    {
        damage = enemyStat.attackDamageRange.CalculateDamage(enemyStat.criticalChance.GetValue(), enemyStat.criticalMultiplier.GetValue());
    }

    public void DrawBow()
    {
        anim.SetBool("Draw", true);
        anim.SetBool("Shoot", false);

        // 생성 지점에서 조준점으로의 방향 벡터 계산
        Vector3 direction = (bowAim.position - arrowSpawnPoint.position).normalized;

        // 방향 벡터를 회전값으로 변환
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        GameObject _arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, targetRotation, arrowSpawnPoint); // 화살 생성
        arrow = _arrow; // 생성된 화살 오브젝트 저장

    }

    public void ShootArrow()
    {
        arrowToTargetDirection = enemyController.target.transform.position - transform.position + new Vector3(0,1f,0);

        anim.SetBool("Draw", false);
        anim.SetBool("Shoot", true);
        
        arrow.GetComponent<ArrowController>().isShooting = true; // 화살이 발사 중임을 표시
        
        StartCoroutine(ResetBow()); // 애니메이션이 끝날 때까지 대기
    }

    public IEnumerator ResetBow()
    {
        yield return new WaitForSeconds(anim.GetNextAnimatorStateInfo(0).length); // 애니메이션이 끝날 때까지 대기
        anim.SetBool("Draw", false);
        anim.SetBool("Shoot", false);
    }

}
