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
        // 1. 현재 플레이어의 데이터 스냅샷을 가져옵니다.
        CharacterData currentData = GetChracterSaveData();
        if (currentData == null)
        {
            Debug.LogError("데이터 스냅샷 생성에 실패하여 저장을 중단합니다.");
            return;
        }

        // 2. 현재 선택된 슬롯에 스냅샷 데이터를 덮어씁니다.
        int slotIndex = _saveLoadManager.GameData.selectedCharacterSlotIndex;
        if (slotIndex >= 0 && slotIndex < _saveLoadManager.GameData.characters.Count)
        {
            _saveLoadManager.GameData.characters[slotIndex] = currentData;

            // 3. 변경된 GameData 전체를 파일에 저장하도록 파일 매니저에 요청합니다.
            _saveLoadManager.SaveGame();
            Debug.Log($"캐릭터 슬롯 {slotIndex}의 데이터가 저장되었습니다.");
        }
        else
        {
            Debug.LogError("선택된 캐릭터 슬롯 인덱스가 유효하지 않습니다.");
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

        // 파일 매니저가 가지고 있는 GameData에서 필요한 부분을 가져옵니다.
        PlayerSaveData dataToLoad = _saveLoadManager.GameData.characters[slotIndex].playerSaveData;

        // PlayerStats의 LoadData 함수를 호출하여 데이터를 적용합니다.
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

        // PlayerStats를 이용해 PlayerSaveData 스냅샷을 만듭니다.
        PlayerSaveData newPlayerSaveData = new PlayerSaveData(playerStats);

        // PlayerSaveData를 포함하는 새로운 CharacterData 컨테이너를 만듭니다.
        CharacterData charData = new CharacterData();
        charData.playerSaveData = newPlayerSaveData;

        return charData;
    }
}

