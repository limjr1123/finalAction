
using UnityEngine;

public enum EnemySkillRange
{
    Range,
    Targeting,
}

public enum EnemySkillType
{
    Attack,
    Heal
}

public interface EnemySkillInterface
{
    void UseSkill(Transform target);
    void Execute();
    bool CanUse();
    float GetCooldown();
    string GetSkillName();
}
