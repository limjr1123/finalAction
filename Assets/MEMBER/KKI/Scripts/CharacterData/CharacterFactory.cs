using GameSave;

public class CharacterFactory
{
    public CharacterData CreateCharacter(string name, JobData _jobData)
    {
        return new CharacterData
        {
            playerSaveData = new PlayerSaveData
            {
                characterName = name,
                jobData = _jobData,
                level = 1,
                currentEXP = 0,
                maxHealth = _jobData.baseHP,
                currentHealth = _jobData.baseHP,
            }
        };
    }
}