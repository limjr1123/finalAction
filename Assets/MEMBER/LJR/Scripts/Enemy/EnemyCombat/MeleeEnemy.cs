using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EnemyAttackStateInfo : 적의 공격 상태를 정의하는 열거형
public enum EnemyAttackStateInfo
{
    Idle,           // 대기 상태
    Windup,         // 공격 준비(선딜레이)
    Impact,         // 타격 판정 구간
    AttackDelay     // 후딜레이
}

public class MeleeEnemy : MonoBehaviour
{
    // 공격 애니메이션과 관련된 데이터
    [SerializeField] List<EnemyAttackData> attacks;
    [SerializeField] GameObject weapon;

    // 공격에 사용할 콜라이더들
    BoxCollider weaponCollider;
    [SerializeField] SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;

    public event Action OnGoHit;
    public event Action OnHitComplete;

    // 캐릭터의 애니메이터 컴포넌트
    Animator anim;

    // 현재 공격 동작(액션) 중인지 여부를 나타냅니다.
    public bool inAction { get; private set; } = false;
    public bool inCounter { get; set; } = false;
    public bool inGetHit { get; set; } = false;

    public EnemyAttackStateInfo attackState;
    public int attacksCount => attacks.Count;

    bool doCombo;
    int comboCounter = 0;

    private void Awake()
    {
        // 컴포넌트가 활성화될 때 애니메이터를 초기화합니다.
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (weapon != null)
        {
            weaponCollider = weapon.GetComponent<BoxCollider>();
        }
        DisableAllCollider();
    }

    // 공격 중이 아닐 때만 Attack 코루틴을 시작합니다.
    public void TryToAttack()
    {
        if (!inAction && !inGetHit)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack(Vector3? attackDir = null)
    {
        inAction = true;
        attackState = EnemyAttackStateInfo.Windup;

        // attacks 리스트에서 애니매이션을 선택
        comboCounter = UnityEngine.Random.Range(0, attacks.Count);

        if (attackDir != null)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir.Value), 360f * Time.deltaTime);
        }
        string animName = attacks[comboCounter].animName;

        anim.CrossFade(animName,0.2f);
        yield return null;  // 프레임 대기하여 애니메이션 정보를 확인

        //GetNextAnimatorStateInfo 애니매이션 상태 정보를 가져옵니다.
        var animState = anim.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= animState.length)
        {
            // normalizedTime을 스킬 실행 시간 백분율로 사용합니다.
            // attacks[comboCounter].impactStartTime은 백분율로 표현됩니다.
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if (attackState == EnemyAttackStateInfo.Windup)
            {
                //if (inCounter) break;
                if (normalizedTime >= attacks[comboCounter].impactStartTime)
                {
                    
                    attackState = EnemyAttackStateInfo.Impact;
                    //콜라이더 켜기
                    EnableHitbox(attacks[comboCounter]);
                }
            }
            else if (attackState == EnemyAttackStateInfo.Impact)
            {
                if (normalizedTime >= attacks[comboCounter].impactEndTime)
                {
                    attackState = EnemyAttackStateInfo.AttackDelay;
                    //콜라이더 끄기
                    DisableAllCollider();
                }
            }
            else if (attackState == EnemyAttackStateInfo.AttackDelay)
            {
                if (doCombo)
                {
                    doCombo = false;
                    StartCoroutine(Attack());
                    yield break;
                }
            }
            yield return null;
        }
        attackState = EnemyAttackStateInfo.Idle;
        comboCounter = 0;
        inAction = false;
    }

    void DisableAllCollider()
    {
        // 초기에는 콜라이더를 비활성화합니다.
        if (weaponCollider != null)
            weaponCollider.enabled = false;
        if (leftHandCollider != null)
            leftHandCollider.enabled = false;
        if (rightHandCollider != null)
            rightHandCollider.enabled = false;
        if (leftFootCollider != null)
            leftFootCollider.enabled = false;
        if (rightFootCollider != null)
            rightFootCollider.enabled = false;
    }

    void EnableHitbox(EnemyAttackData attack)
    {
        switch (attack.hitboxToUse)
        {
            case AttackHitbox.LeftHand:
                leftHandCollider.enabled = true;
                break;
            case AttackHitbox.RightHand:
                rightHandCollider.enabled = true;
                break;
            case AttackHitbox.TwoHand:
                leftHandCollider.enabled = true;
                rightHandCollider.enabled = true;
                break;
            case AttackHitbox.Weapon:
                weaponCollider.enabled = true;
                break;
            case AttackHitbox.LeftFoot:
                    leftFootCollider.enabled = true;
                break;
            case AttackHitbox.RightFoot:
                    rightFootCollider.enabled = true;
                break;
            default:
                break;
        }
    }
}
