using UnityEngine;
using GameSave;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public PlayerSaveData SaveChracterData()
    {
        // 1. 현재 플레이어의 데이터 스냅샷을 가져옵니다.
        PlayerSaveData currentCharacterData = GetChracterSaveData();
        if (currentCharacterData == null)
        {
            Debug.LogError("데이터 스냅샷 생성에 실패하여 저장을 중단합니다.");
            return null;
        }

        return currentCharacterData;
    }

    public void LoadCharacterSaveData(PlayerSaveData playerSaveData)
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null) return;

        PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
        if (playerStats == null) return;

        // PlayerStats의 LoadData 함수를 호출하여 데이터를 적용합니다.
        playerStats.LoadData(playerSaveData);
    }

    private PlayerSaveData GetChracterSaveData()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player null");
            return null;
        }

        PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats null");
            return null;
        }

        // PlayerStats를 이용해 PlayerSaveData 스냅샷을 만듭니다.
        PlayerSaveData newPlayerSaveData = new PlayerSaveData(playerStats);

        return newPlayerSaveData;
    }
}

