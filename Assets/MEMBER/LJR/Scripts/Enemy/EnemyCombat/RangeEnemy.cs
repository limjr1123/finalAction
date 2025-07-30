using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    EnemyController enemyController;

    [SerializeField] GameObject weaponObj;      // 원거리 공격에 사용할 무기 오브젝트
    [SerializeField] public BowController bow;  // bow 컨트롤러

    [SerializeField] EnemyAttackData draw;  // 공격 애니메이션과 관련된 데이터
    [SerializeField] EnemyAttackData shoot; // 공격 애니메이션과 관련된 데이터

    Animator anim;

    public bool inAction { get; private set; } = false; // 현재 공격 동작 중인지 여부  
    public bool inGetHit { get; set; } = false; // 현재 피격 상태인지 여부
    public bool isReloading { get; set; } = false; // 재장전 완료 여부

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

    public void TryToAttack()
    {
        if (!inAction && !inGetHit)
        {
            StartCoroutine(ShootArrow());
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

    IEnumerator ShootArrow(Vector3? attackDir = null)
    {
        inAction = true;

        if (draw != null)
        {
            if (!isReloading)
            {
                isReloading = true;
                bow.DrawBow(); // 활을 당기는 애니메이션 실행
            }
            anim.CrossFade(draw.animName, 0.2f);    
            yield return new WaitUntil(() => isReloading);
            attackState = EnemyAttackStateInfo.Windup;
        }
        else
        {
            attackState = EnemyAttackStateInfo.Windup;
        }

        anim.CrossFade(shoot.animName, 0.2f);
        yield return null;  // 프레임 대기하여 애니메이션 정보를 확인

        var animState = anim.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer < animState.length)
        {
            timer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
    }
}
