using System.Collections;
using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    EnemyController enemyController;

    [SerializeField] GameObject weaponObj;          // 원거리 공격에 사용할 무기 오브젝트
    [SerializeField] public BowController bow;      // bow 컨트롤러

    [SerializeField] EnemyAttackData draw;  // 공격 애니메이션과 관련된 데이터
    [SerializeField] EnemyAttackData shoot; // 공격 애니메이션과 관련된 데이터

    Animator anim;

    public bool inAction { get; private set; } = false; // 현재 공격 동작 중인지 여부  
    public bool isShooting { get; set; } = false; // 공격 중인 상태인지 여부

    public EnemyAttackStateInfo attackState;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        WeaponSetting();
    }

    private void Update()
    {
        if (attackState == EnemyAttackStateInfo.Windup)
            enemyController.TargetChaseDirection(enemyController.target.transform.position - transform.position);
    }
    public void TryToAttack()
    {
        if (!inAction && !enemyController.inGetHit)
        {
            StartCoroutine(RangeAttack());
            inAction = true;
        }
    }

    void WeaponSetting()
    {
        if (weaponObj.GetComponent<BowController>())
        {
            bow = weaponObj.GetComponent<BowController>();
            return;
        }
    }

    IEnumerator RangeAttack(Vector3? attackDir = null)
    {
        //Debug.Log("RangeEnemy Attack Start");
        inAction = true;
        isShooting = true; // 공격 중인 상태로 설정

        if (draw == null)
            attackState = EnemyAttackStateInfo.Impact;
        else
            attackState = EnemyAttackStateInfo.Windup;

        while (isShooting)
        {
            if (enemyController.inGetHit || enemyController.stats.currentHealth <= 0)
            {
                isShooting = false; // 피격 상태면 공격 중지
                inAction = false;
                attackState = EnemyAttackStateInfo.Idle;
                break;
            }

            if (attackState == EnemyAttackStateInfo.Windup)
            {
                anim.CrossFade(draw.animName, 0.2f);

                yield return new WaitForSeconds(draw.impactStartTime);
                bow.DrawBow(); // 활을 당기는 애니메이션 실행

                yield return new WaitForSeconds(draw.impactEndTime - draw.impactStartTime);
                attackState = EnemyAttackStateInfo.AttackDelay; // 애니메이션이 끝나면 Impact 상태로 변경
            }
            else if (attackState == EnemyAttackStateInfo.AttackDelay)
            {
                yield return new WaitForSeconds(0.25f);
                attackState = EnemyAttackStateInfo.Impact;
            }
            else if (attackState == EnemyAttackStateInfo.Impact)
            {
                bow.ShootArrow();
                anim.CrossFade(shoot.animName, 0.2f);

                yield return new WaitForSeconds(shoot.impactEndTime);
                isShooting = false; // 공격상태 종료
            }
        }
        attackState = EnemyAttackStateInfo.Idle;
        inAction = false;
    }
}
