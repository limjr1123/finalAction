using GameSave;

public class CharacterFactory
{
    public CharacterData CreateCharacter(string name, JobData _jobData)
    {
        return new CharacterData
        {
            // 캐릭터 생성시에 처음 데이터를 여기서 넣어주기.
            playerSaveData = new PlayerSaveData
            {
                characterName = name,
                characterJob = _jobData.jobName,
                level = 1,
                currentEXP = 0,

                maxHealth = _jobData.baseHP,
                currentHealth = _jobData.baseHP,
                attackDamage = _jobData.baseAttack,
                defense = _jobData.baseDefance
            }
        };
    }
}