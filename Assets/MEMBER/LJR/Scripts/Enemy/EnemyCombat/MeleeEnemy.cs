using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeEnemy : MonoBehaviour
{
    EnemyController enemyController;

    // 공격 애니메이션과 관련된 데이터
    [SerializeField] List<EnemyAttackData> attacks; // 공격 애니메이션 데이터
    [SerializeField] List<EnemyAttackData> skills;  // 스킬 애니메이션 데이터

    [SerializeField] GameObject weapon;

    [SerializeField] EnemySkillInterface[] skillList;
    EnemySkillInterface selectedSkill;

    // 공격에 사용할 콜라이더들
    BoxCollider weaponCollider;
    [SerializeField] SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;

    public event Action OnGoHit;
    public event Action OnHitComplete;

    [SerializeField] AudioClip slash;
    [SerializeField] AudioClip swing;
    [field: SerializeField] public HitEffectType hitEffectType { get; private set; }
    Dictionary<HitEffectType, AudioClip> attackAudioDict = new Dictionary<HitEffectType, AudioClip>();

    // 캐릭터의 애니메이터 컴포넌트
    Animator anim;

    public bool isParry { get; set; } = false; // 패링 상태 여부

    // 현재 공격 동작(액션) 중인지 여부를 나타냅니다.
    public bool inAction { get; private set; } = false;
    public bool inCounter { get; set; } = false;

    public EnemyAttackStateInfo attackState;
    public int attacksCount => attacks.Count;

    bool doCombo;
    int comboCounter = 0;

    public float skillCooldownTimer = 0f;

    private void Awake()
    {
        // 컴포넌트가 활성화될 때 애니메이터를 초기화합니다.
        anim = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>();
        skillList = GetComponents<EnemySkillInterface>();
    }

    private void Start()
    {
        if (weapon != null)
        {
            weaponCollider = weapon.GetComponent<BoxCollider>();
        }
        DisableAllCollider();

        attackAudioDict.Add(HitEffectType.Slash, slash);
        attackAudioDict.Add(HitEffectType.Hit, swing);
    }

    private void Update()
    {
        skillCooldownTimer -= Time.deltaTime;
    }

    // 공격 중이 아닐 때만 Attack 코루틴을 시작합니다.
    public void TryToAttack()
    {
        if (!inAction && !enemyController.inGetHit)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack(Vector3? attackDir = null)
    {
        inAction = true;
        attackState = EnemyAttackStateInfo.Windup;

        if (attackDir != null)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir.Value), 360f * Time.deltaTime);
        }

        EnemyAttackData attackData;

        if (!enemyController.isSkillUse && skillCooldownTimer <= 0)
        {
            // skills 리스트에서 애니매이션을 선택
            comboCounter = UnityEngine.Random.Range(0, skills.Count);
            attackData = skills[comboCounter];
        }
        else
        {        
            // attacks 리스트에서 애니매이션을 선택
            comboCounter = UnityEngine.Random.Range(0, attacks.Count);
            attackData = attacks[comboCounter];
        }
        string animName = attackData.animName;

        anim.CrossFade(animName, 0.2f);
        yield return null;  // 프레임 대기하여 애니메이션 정보를 확인

        // 공격 시작 시 사운드 재생
        SoundManager.Instance.PlaySkillSFX(attackAudioDict[hitEffectType]);
        //GetNextAnimatorStateInfo 애니매이션 상태 정보를 가져옵니다.
        var animState = anim.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= animState.length)
        {
            // normalizedTime을 스킬 실행 시간 백분율로 사용합니다.
            // attacks[comboCounter].impactStartTime은 백분율로 표현됩니다.
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            // 스킬 사용 시 쿨타임 타이머 설정
            if(attackData.isSkill)
            {
                enemyController.isSkillUse = true;
                skillCooldownTimer = attackData.skillCoolDown;
            }

            if (attackState == EnemyAttackStateInfo.Windup)
            {
                //if (inCounter) break;
                if (normalizedTime >= attackData.impactStartTime)
                {
                    isParry = attacks[comboCounter].isParry; // 패링 가능한 공격인지 확인
                    attackState = EnemyAttackStateInfo.Impact;
                    //콜라이더 켜기
                    EnableHitbox(attackData, normalizedTime);
                }
            }
            else if (attackState == EnemyAttackStateInfo.Impact)
            {
                if (normalizedTime >= attackData.impactEndTime)
                {
                    if (attackData.isSkill)
                    {
                        Debug.Log( "스킬보기 : "+ skillList[comboCounter].GetSkillName());
                        skillList[comboCounter].UseSkill(transform);
                    }
                    attackState = EnemyAttackStateInfo.AttackDelay;
                    //콜라이더 끄기
                    DisableAllCollider(normalizedTime);
                    isParry = false; // 초기화
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
        enemyController.isSkillUse = false;
    }

    void DisableAllCollider(float? normalizedTime = 0)
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

    void EnableHitbox(EnemyAttackData attack, float normalizedTime)
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
            case AttackHitbox.None:
                break;
            default:
                break;
        }
    }
}
