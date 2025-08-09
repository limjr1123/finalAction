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
                defense = _jobData.baseDefance,

                // 위치 
                savePos = new UnityEngine.Vector3(78.3310013f, -11.0290003f, 54.4690018f),
                saveRot = new UnityEngine.Vector3(0, 0, 0),
            },
            gold = 1000,
            // 인벤토리 처음 아이템,
            // 퀘스트 처음 퀘스트,
            // 등등
        };
    }
}