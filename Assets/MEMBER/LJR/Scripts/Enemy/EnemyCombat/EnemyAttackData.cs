using UnityEngine;

//Enemy 공격 데이터를 정의하는 스크립트입니다.
[CreateAssetMenu(menuName = "Combat System/Create a new Attack")]
public class EnemyAttackData : ScriptableObject
{
    [field: SerializeField] public string animName { get; private set; }
    [field: SerializeField] public AttackHitbox hitboxToUse { get; private set; }
    [field: SerializeField] public float impactStartTime { get; private set; }
    [field: SerializeField] public float impactEndTime { get; private set; }
    [field: SerializeField] public bool isParry { get; private set; } // 패링 가능한 공격인지 여부
}

// 공격 시 사용할 히트박스 종류를 정의하는 열거형입니다.
public enum AttackHitbox { LeftHand, RightHand, TwoHand, Weapon, LeftFoot, RightFoot }
