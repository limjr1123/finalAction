using System.Collections.Generic;
using UnityEngine;
public enum SkillType { Attack, Buff } // 스킬 종류
public enum StatType { Str, Dex, Int, defense, magicDefense }  // 버프에 쓰일 스탯

[System.Serializable]
public struct StatBuff
{
    public StatType statToBuff;
    public int amount;
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("스킬 정보")]
    public SkillType skillType;
    public string skillName;
    public float cooldown;
    public int manaCost;
    public string animationTriggerName;

    [Header("공격스킬 정보")]
    public float damageMultiplier; // 배율

    [Header("버프스킬 정보")]
    public List<StatBuff> buffs = new List<StatBuff>(); // 버프 스탯 리스트
    public float buffDuration;  // 지속시간

}
