using UnityEngine;

// 공격 시 사용할 히트박스 종류를 정의하는 열거형입니다.
public enum AttackHitbox { LeftHand, RightHand, TwoHand, Weapon, LeftFoot, RightFoot, None }
public enum AttackCount { Single, Multi }
//Enemy 공격 데이터를 정의하는 스크립트입니다.
[CreateAssetMenu(menuName = "Combat System/Create a new Attack")]
public class EnemyAttackData : ScriptableObject
{
    public AttackCount attackCount; // 공격 횟수 (단일 공격 또는 연타 공격)

    [Header("단일 공격")]
    [field: SerializeField] public string animName { get; private set; }
    [field: SerializeField] public AttackHitbox hitboxToUse { get; private set; }
    [field: SerializeField] public float impactStartTime { get; private set; }  // 타격 판정이 시작되는 시간 (원거리 공격의 경우 사용되지 않음)
    [field: SerializeField] public float impactEndTime { get; private set; }    // 타격 판정이 끝나는 시간 (원거리 공격의 경우 애니메이션 종료 시간을 입력)
    [field: SerializeField] public bool isParry { get; private set; } // 패링 가능한 공격인지 여부

    [Header("연타 공격")]
    [field: SerializeField] public AttackPhase[] attackPhases { get; private set; } // 공격 단계 리스트

    [Header("특수 스킬")]
    [field: SerializeField] public bool isSkill { get; private set; } = false; // 스킬 여부
    [field: SerializeField] public float skillCoolDown { get; private set; } = 0; // 스킬 쿨타임 (스킬이 아닐 경우 0으로 설정)
    [field: SerializeField] public float skillCost { get; private set; } = 0; // 스킬 사용 비용 (스킬이 아닐 경우 0으로 설정)

}

[System.Serializable]
public struct AttackPhase
{
    [SerializeField] public string animName;          // 해당 타격의 애니메이션 이름
    [SerializeField] public AttackHitbox hitboxToUse; // 사용할 히트박스
    [SerializeField] public float impactStartTime;    // 타격 판정이 시작되는 시간
    [SerializeField] public float impactEndTime;      // 타격 판정이 끝나는 시간
    [SerializeField] public bool isParry;             // 패링 가능한 공격인지 여부
    [SerializeField] public float attackZAngle;        // 공격 방향 (회전값)
}
