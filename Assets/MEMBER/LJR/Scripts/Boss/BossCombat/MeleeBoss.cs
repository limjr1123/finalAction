using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBoss : MonoBehaviour
{
    EnemyStat bossStat;

    // 공격 애니메이션과 관련된 데이터
    [Header("Melee Boss Attack List")]
    [SerializeField] List<EnemyAttackData> attacks;
    [SerializeField] List<EnemyAttackData> unblockableAttacks; // 방어가 불가능한 공격
    [SerializeField] List<EnemyAttackData> rageAttack;

    [Header("Melee Boss Special Attack Info")]
    [SerializeField] Transform rightAttackImpactPosition;   // 가드불가 공격의 임팩트 포지션(오른손)
    [SerializeField] Transform leftAttackImpactPosition;    // 가드불가 공격의 임팩트 포지션(왼손)
    Transform attackImpactPosition; // 공격 임팩트 포지션

    [SerializeField] GameObject swordWindPrefab;    // 검풍 이펙트 프리팹
    [SerializeField] Transform swordWindAim;        // 검풍 이펙트 진행 방향
    [SerializeField] float swordWindZAngle;         // 검풍 이펙트 회전값

    [Header("Melee Boss Hitbox Info")]
    [SerializeField] GameObject weapon; // 공격에 사용할 무기 오브젝트
    BoxCollider weaponCollider;
    [SerializeField] Collider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;

    [SerializeField] GameObject warningSignPrefab; // 경고표시

    // 캐릭터의 애니메이터 컴포넌트
    Animator anim;
    int attackIndex = 0;
    int unblockableAttackIndex = 0;
    public bool isParry { get; set; } = false; // 패링 상태 여부
    public bool inAction { get; private set; } = false;

    public EnemyAttackStateInfo attackState;

    private void Awake()
    {
        // 컴포넌트가 활성화될 때 애니메이터를 초기화합니다.
        anim = GetComponent<Animator>();
        bossStat = GetComponent<EnemyStat>();
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
            SetAttack();
        }
    }

    private void SetAttack()
    {
        if (Random.Range(0, 3) == 0)
        {
            StartCoroutine(Attack());
        }
        else
        {
            StartCoroutine(UnblockableAttack());
        }
    }

    IEnumerator Attack(Vector3? attackDir = null)
    {
        inAction = true;
        attackState = EnemyAttackStateInfo.Windup;

        // attacks 리스트에서 애니메이션을 선택
        attackIndex = Random.Range(0, attacks.Count);

        if (attackDir != null)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir.Value), 360f * Time.deltaTime);
        }
        string animName = attacks[attackIndex].animName;
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
        if (attackState == EnemyAttackStateInfo.AttackDelay)
        {
            attackState = EnemyAttackStateInfo.Idle;
            inAction = false;
        }
    }

    IEnumerator UnblockableAttack(Vector3? attackDir = null)
    {
        inAction = true;
        attackState = EnemyAttackStateInfo.Charge;

        unblockableAttackIndex = Random.Range(0, unblockableAttacks.Count);
        if (attackDir != null)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir.Value), 360f * Time.deltaTime);
        }

        int currentPhaseIndex = 0; // 현재 처리 중인 attackPhase 인덱스
        var animName = unblockableAttacks[unblockableAttackIndex].attackPhases[currentPhaseIndex].animName;

        anim.CrossFade(animName, 0.2f);
        yield return null; // 프레임 대기하여 애니메이션 정보를 확인

        float timer = 0f;
        var animState = anim.GetNextAnimatorStateInfo(1);

        // 공격 임팩트 포인트 설정 및 이펙트 생성
        AttackImpactPointCheck(currentPhaseIndex);
        HitEffectManager.Instance.EffectCreate(attackImpactPosition, HitEffectType.AttackReady);

        StartCoroutine(WarningSignCreate());

        while (timer <= animState.length && currentPhaseIndex < unblockableAttacks[unblockableAttackIndex].attackPhases.Length)
        {
            Debug.Log("phases :"+unblockableAttacks[unblockableAttackIndex].attackPhases.Length);
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            var currentPhase = unblockableAttacks[unblockableAttackIndex].attackPhases[currentPhaseIndex];

            if (attackState == EnemyAttackStateInfo.Charge)
            {
                if (timer >= animState.length) // 애니메이션이 끝나면
                {
                    attackState = EnemyAttackStateInfo.Windup;
                    currentPhaseIndex++;
                    animName = unblockableAttacks[unblockableAttackIndex].attackPhases[currentPhaseIndex].animName;
                    anim.CrossFade(animName, 0.2f);
                    yield return null; // 프레임 대기하여 애니메이션 정보를 확인
                    timer = 0f; // 타이머 초기화
                    animState = anim.GetNextAnimatorStateInfo(1); // 다음 애니메이션 상태 정보 갱신
                }
            }
            else if (attackState == EnemyAttackStateInfo.Windup)
            {
                if (normalizedTime >= currentPhase.impactStartTime)
                {
                    attackState = EnemyAttackStateInfo.Impact;
                    EnableHitbox(currentPhase); // 현재 phase의 collider 켜기
                    swordWindZAngle = currentPhase.attackZAngle; // 검풍 회전값 설정
                    SwordWindCreate(); // 검풍 이펙트 생성
                }
            }
            else if (attackState == EnemyAttackStateInfo.Impact)
            {
                if (normalizedTime >= currentPhase.impactEndTime)
                {
                    attackState = EnemyAttackStateInfo.Windup;
                    DisableAllCollider(); // collider 끄기
                    currentPhaseIndex++; // 다음 phase로 이동
                    Debug.Log("Unblockable Attack Phase Index: " + currentPhaseIndex);
                    if (currentPhaseIndex > unblockableAttacks[unblockableAttackIndex].attackPhases.Length - 1)
                    {
                        attackState = EnemyAttackStateInfo.Idle;
                        inAction = false;
                        break;
                    }
                    animName = unblockableAttacks[unblockableAttackIndex].attackPhases[currentPhaseIndex].animName;
                    Debug.Log(animName);
                    anim.CrossFade(animName, 0.2f);
                    yield return null; // 프레임 대기하여 애니메이션 정보를 확인
                    timer = 0f; // 타이머 초기화
                    animState = anim.GetNextAnimatorStateInfo(1); // 다음 애니메이션 상태 정보 갱신
                    continue;
                }
            }
            yield return null;
        }

    }

    private IEnumerator WarningSignCreate()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject warning = Instantiate(warningSignPrefab, swordWindAim.position, swordWindAim.rotation); // 경고 표시 생성
        warning.transform.Translate(Vector3.up * -1.5f); // 경고 표시를 약간 아래로 이동
        //warning.transform.position = new Vector3(warning.transform.position.x, 0, warning.transform.position.z);
        yield return new WaitForSeconds(0.2f); // 경고 표시가 보이는 시간
        Destroy(warning); // 경고 표시 제거
    }

    private void SwordWindCreate()
    {
        
        int damage = bossStat.attackDamageRange.GetRandomDamage();
        GameObject wind = Instantiate(swordWindPrefab, swordWindAim.position, swordWindAim.rotation); // 검풍 이펙트 생성
        wind.GetComponent<SwordWindController>().InitializeSwordWind(swordWindAim, swordWindZAngle, damage);
    }

    private void AttackImpactPointCheck(int phaseIndex)
    {
        switch (unblockableAttacks[unblockableAttackIndex].attackPhases[phaseIndex].hitboxToUse)
        {
            case AttackHitbox.LeftHand:
                attackImpactPosition = leftAttackImpactPosition;
                break;
            case AttackHitbox.RightHand:
                attackImpactPosition = rightAttackImpactPosition;
                break;
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
