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
        // 1. ���� �÷��̾��� ������ �������� �����ɴϴ�.
        PlayerSaveData currentCharacterData = GetChracterSaveData();
        if (currentCharacterData == null)
        {
            Debug.LogError("������ ������ ������ �����Ͽ� ������ �ߴ��մϴ�.");
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

        // PlayerStats�� LoadData �Լ��� ȣ���Ͽ� �����͸� �����մϴ�.
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

        // PlayerStats�� �̿��� PlayerSaveData �������� ����ϴ�.
        PlayerSaveData newPlayerSaveData = new PlayerSaveData(playerStats);

        return newPlayerSaveData;
    }
}

