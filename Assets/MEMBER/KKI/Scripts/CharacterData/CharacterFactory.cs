using GameSave;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory
{
    private const string StartQuestId = "TalkToChief1";

    public CharacterData CreateCharacter(string name, JobData j)
    {
        var ps = new PlayerSaveData
        {
            characterName = name,
            characterJob = j.jobName,

            level = j.startLevel,
            currentEXP = 0,

            // 현재값(HP/MP/스태미나)은 보통 최대치로 시작
            currentHealth = j.baseHP,
            currentMana = j.baseMP,
            currentStamina = j.baseStamina,

            // 기본 능력치
            maxHealth = j.baseHP,
            maxMana = j.baseMP,
            manaRegen = j.baseManaRegen,
            maxStamina = j.baseStamina,
            staminaRegen = j.baseStaminaRegen,
            maxEXP = j.startMaxEXP,
            defense = j.baseDefense,
            magicDefense = j.baseMagicDefense,
            Str = j.baseStr,
            Dex = j.baseDex,
            Int = j.baseInt,

            // 이동/전투/크리 관련
            moveSpeed = j.baseMoveSpeed,
            sprintSpeed = j.baseSprintSpeed,
            attackDamage = j.baseAttack,
            magicDamage = j.baseMagicDamage,
            attackSpeed = j.baseAttackSpeed,
            criRate = j.baseCriRate,
            criDamage = j.baseCriDamage,
            criResist = j.baseCriResist,

            // 시작 위치
            savePos = new Vector3(78.3310013f, -11.0290003f, 54.4690018f),
            saveRot = Vector3.zero,
        };

        return new CharacterData
        {
            playerSaveData = ps,
            gold = 1000,
            questSaveData = new QuestSaveData
            {
                completedQuests = new List<string>(),
                activeQuests = new List<string> { StartQuestId }
            },
            // inventorySaveData는 필요 시 채우기
        };
    }
}