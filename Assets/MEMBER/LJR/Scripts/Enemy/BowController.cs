using System.Collections;
using UnityEngine;


// BowController : 적의 활을 제어하는 클래스 활 장전, 조준, 발사 등의 기능을 포함. 활에 부착된 컴포넌트
public class BowController : MonoBehaviour
{
    public Animator anim { get; set; }      // 무기 애니메이터
    [SerializeField] Transform bowAim;              // 활의 조준 위치
    [SerializeField] Transform arrowSpawnPoint;     // 화살이 생성될 위치
    private ArrowController arrowController;        // 생성된 화살 오브젝트

    public Vector3 bowDirection;            // 화살 장전 시 활의 방향 벡터(활의 애니메이션과 화살의 방향을 동기화하기 위함)
    public Vector3 arrowToTargetDirection;  // 타겟조준 방향 벡터

    EnemyController enemyController;    // 적 컨트롤러
    EnemyStat enemyStat;                // 적의 스탯 컴포넌트

    public int damage; // 화살 데미지(화살이 발사될 때 Arrow로 넘겨줌)

    [SerializeField] private AudioClip bowDrawSFX; // 활 장전 사운드
    [SerializeField] private AudioClip bowShootSFX; // 활 발사 사운드

    void Awake()
    {
        anim = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        enemyController = GetComponentInParent<EnemyController>();
        enemyStat = GetComponentInParent<EnemyStat>();
    }

    private void Update()
    {
        if (enemyController.inGetHit)
        {
            ResetBow();
            ArrowPool.Instance.ReturnArrow(arrowController.gameObject); // 피격 상태일 때 화살 제거
        }
    }

    // 활 장전 및 조준
    public void DrawBow()
    {
        // 적의 공격력 계산
        damage = enemyStat.attackDamageRange.CalculateDamage(enemyStat.criticalChance.GetValue(), enemyStat.criticalMultiplier.GetValue()); 

        anim.SetBool("Draw", true);
        anim.SetBool("Shoot", false);

        SoundManager.Instance.PlaySkillSFX(bowDrawSFX);

        bowDirection = (bowAim.position - arrowSpawnPoint.position).normalized;    // 활시위에서 조준점으로의 방향 벡터 계산
        Quaternion targetRotation = Quaternion.LookRotation(bowDirection);     // 방향 벡터를 회전값으로 변환

        GameObject _arrow = ArrowPool.Instance.GetArrow(arrowSpawnPoint.position, targetRotation, arrowSpawnPoint); // 화살 생성
        arrowController = _arrow.GetComponent<ArrowController>(); // 생성된 화살 오브젝트 저장
        arrowController.hitBox.enabled = false; // 화살의 HitBox 비활성화
        arrowController.bow = this;     // BowController 설정
        arrowController.SetDamage();    // 데미지 설정
    }

    // 화살 발사
    public void ShootArrow()
    {
        arrowToTargetDirection = enemyController.target.transform.position - transform.position + new Vector3(0,1f,0); // 조준 방향 벡터 계산

        SoundManager.Instance.PlaySkillSFX(bowShootSFX);

        anim.SetBool("Draw", false);
        anim.SetBool("Shoot", true);

        arrowController.ShootingArrow(arrowToTargetDirection); // 화살 발사
        StartCoroutine(ResetBow()); // 애니메이션이 끝날 때까지 대기
    }

    // 활을 초기 상태로 리셋
    public IEnumerator ResetBow()
    {
        yield return new WaitForSeconds(anim.GetNextAnimatorStateInfo(0).length); // 애니메이션이 끝날 때까지 대기
        anim.SetBool("Draw", false);
        anim.SetBool("Shoot", false);
    }

}
