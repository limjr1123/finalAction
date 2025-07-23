using UnityEngine;
using GameSave;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private GameDataSaveLoadManager _saveLoadManager;

    protected override void Awake()
    {
        base.Awake();
        _saveLoadManager = GameDataSaveLoadManager.Instance;
    }

    public void SaveChracterData()
    {
        // 1. ���� �÷��̾��� ������ �������� �����ɴϴ�.
        CharacterData currentData = GetChracterSaveData();
        if (currentData == null)
        {
            Debug.LogError("������ ������ ������ �����Ͽ� ������ �ߴ��մϴ�.");
            return;
        }

        // 2. ���� ���õ� ���Կ� ������ �����͸� ����ϴ�.
        int slotIndex = _saveLoadManager.GameData.selectedCharacterSlotIndex;
        if (slotIndex >= 0 && slotIndex < _saveLoadManager.GameData.characters.Count)
        {
            _saveLoadManager.GameData.characters[slotIndex] = currentData;

            // 3. ����� GameData ��ü�� ���Ͽ� �����ϵ��� ���� �Ŵ����� ��û�մϴ�.
            _saveLoadManager.SaveGame();
            Debug.Log($"ĳ���� ���� {slotIndex}�� �����Ͱ� ����Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError("���õ� ĳ���� ���� �ε����� ��ȿ���� �ʽ��ϴ�.");
        }
    }

    public void LoadCharacterSaveData()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null) return;

        PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
        if (playerStats == null) return;

        int slotIndex = _saveLoadManager.GameData.selectedCharacterSlotIndex;
        if (slotIndex < 0 || slotIndex >= _saveLoadManager.GameData.characters.Count) return;

        // ���� �Ŵ����� ������ �ִ� GameData���� �ʿ��� �κ��� �����ɴϴ�.
        PlayerSaveData dataToLoad = _saveLoadManager.GameData.characters[slotIndex].playerSaveData;

        // PlayerStats�� LoadData �Լ��� ȣ���Ͽ� �����͸� �����մϴ�.
        playerStats.LoadData(dataToLoad);
    }

    private CharacterData GetChracterSaveData()
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

        // PlayerSaveData�� �����ϴ� ���ο� CharacterData �����̳ʸ� ����ϴ�.
        CharacterData charData = new CharacterData();
        charData.playerSaveData = newPlayerSaveData;

        return charData;
    }
}

