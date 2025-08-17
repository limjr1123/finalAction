using GameSave;
using System.Collections.Generic;


public class CharacterFactory
{
    private const string StartQuestId = "TalkToChief1";

    public CharacterData CreateCharacter(string name, JobData _jobData)
    {
        return new CharacterData
        {
            playerSaveData = new PlayerSaveData
            {
                characterName = name,
                characterJob = _jobData.jobName,
                level = 1,
                currentEXP = 0,
                maxHealth = _jobData.baseHP,
                currentHealth = _jobData.baseHP,
                attackDamage = _jobData.baseAttack,
                defense = _jobData.baseDefance,
                savePos = new UnityEngine.Vector3(78.3310013f, -11.0290003f, 54.4690018f),
                saveRot = UnityEngine.Vector3.zero,
            },
            gold = 1000,

            questSaveData = new QuestSaveData
            {
                completedQuests = new List<string>(),
                activeQuests = new List<string> { StartQuestId }
            },

            // 시작 인벤토리도 여기서 채워 넣을 수 있음
        };
    }
}