using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBoss : MonoBehaviour
{
    BossController boss;
    // 공격 애니메이션과 관련된 데이터
    [SerializeField] List<EnemyAttackData> attacks;
    [SerializeField] List<EnemyAttackData> unblockableAttacks; // 방어가 불가능한 공격
    [SerializeField] List<EnemyAttackData> rageAttack;
    [SerializeField] GameObject weapon;

    // 공격에 사용할 콜라이더들
    BoxCollider weaponCollider;
    [SerializeField] Collider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;


    // 캐릭터의 애니메이터 컴포넌트
    Animator anim;
    int attackIndex = 0;

    public bool isParry { get; set; } = false; // 패링 상태 여부
    public bool inAction { get; private set; } = false;

    public EnemyAttackStateInfo attackState;

    private void Awake()
    {
        // 컴포넌트가 활성화될 때 애니메이터를 초기화합니다.
        anim = GetComponent<Animator>();
        boss = GetComponent<BossController>();
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
        if (!inAction)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack(Vector3? attackDir = null)
    {
        inAction = true;
        attackState = EnemyAttackStateInfo.Windup;

        // attacks 리스트에서 애니메이션을 선택
        attackIndex = UnityEngine.Random.Range(0, attacks.Count);

        if (attackDir != null)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir.Value), 360f * Time.deltaTime);
        }
        string animName = attacks[attackIndex].animName;
        Debug.Log("animName: " + animName);
        anim.CrossFade(animName, 0.2f);
        yield return null; // 프레임 대기하여 애니메이션 정보를 확인

        // 애니메이션 상태 정보 가져오기
        var animState = anim.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        int currentPhaseIndex = 0; // 현재 처리 중인 attackPhase 인덱스

        if (attacks[attackIndex].attackCount == AttackCount.Multi)
        {
            while (timer <= animState.length && currentPhaseIndex < attacks[attackIndex].attackPhases.Length)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / animState.length;

                var currentPhase = attacks[attackIndex].attackPhases[currentPhaseIndex];

                if (attackState == EnemyAttackStateInfo.Windup)
                {
                    if (normalizedTime >= currentPhase.impactStartTime)
                    {
                        isParry = currentPhase.isParry; // 패링 가능 여부 설정
                        attackState = EnemyAttackStateInfo.Impact;
                        EnableHitbox(currentPhase); // 현재 phase의 collider 켜기
                    }
                }
                else if (attackState == EnemyAttackStateInfo.Impact)
                {
                    if (normalizedTime >= currentPhase.impactEndTime)
                    {
                        attackState = EnemyAttackStateInfo.Windup;
                        DisableAllCollider(); // collider 끄기
                        isParry = false; // 패링 초기화
                        currentPhaseIndex++; // 다음 phase로 이동
                    }
                }
                yield return null;
            }
            attackState = EnemyAttackStateInfo.AttackDelay;
        }
        else
        {
            // Single 공격 로직 
            while (timer <= animState.length)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / animState.length;

                if (attackState == EnemyAttackStateInfo.Windup)
                {
                    if (normalizedTime >= attacks[attackIndex].impactStartTime)
                    {
                        isParry = attacks[attackIndex].isParry;
                        attackState = EnemyAttackStateInfo.Impact;
                        EnableHitbox(attacks[attackIndex]);
                    }
                }
                else if (attackState == EnemyAttackStateInfo.Impact)
                {
                    if (normalizedTime >= attacks[attackIndex].impactEndTime)
                    {
                        attackState = EnemyAttackStateInfo.AttackDelay;
                        DisableAllCollider();
                        isParry = false;
                    }
                }
                yield return null;
            }
        }
        if(attackState == EnemyAttackStateInfo.AttackDelay)
        {
            attackState = EnemyAttackStateInfo.Idle;
            inAction = false;
        }
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

    void EnableHitbox(AttackPhase attack)
    {
        switch (attack.hitboxToUse)
        {
            case AttackHitbox.LeftHand:
                Debug.Log("LeftHand Collider Enabled");
                leftHandCollider.enabled = true;
                break;
            case AttackHitbox.RightHand:
                Debug.Log("RightHand Collider Enabled");
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
