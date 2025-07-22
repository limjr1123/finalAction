using GameSave;

public class CharacterFactory
{
    public CharacterData CreateCharacter(string name, JobData _jobData)
    {
        return new CharacterData
        {
            characterName = name,
            jobData = _jobData,
            level = 1,
            hp = _jobData.baseHP,
            exp = 0,
            inventorySaveData = new InventorySaveData(),
            questSaveData = new QuestSaveData(),
            // 나머지
        };
    }
}