using System.Collections.Generic;
using System.IO;
using GameSave;
using UnityEngine;

namespace GameSave
{
    [System.Serializable]
    public class GameData
    {
        public int selectedCharacterSlotIndex; // 현재 선택된 캐릭터 슬롯
        public List<CharacterData> characters = new();
        public UserSettings userSettings = new();

        // 기타 다른 정보들(업적, )
    }

    [System.Serializable]
    public class CharacterData
    {
        public PlayerSaveData playerSaveData = new();
        public InventorySaveData inventoryData = new();
        public QuestSaveData questData = new();
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<InventorySlotSaveData> items = new();
    }

    [System.Serializable]
    public class InventorySlotSaveData
    {
        public string itemID;
        public int count;
    }

    [System.Serializable]
    public class QuestSaveData
    {
        public List<string> completedQuests = new();
        public List<string> currentQuests = new();
        // 필요한 정보 추가
    }

    [System.Serializable]
    public class UserSettings
    {
        public float bgmVolume;
        public float sfxVolume;
        public int screenResolution;
        public bool isFullScreen;
    }
}


public class GameDataSaveLoadManager : Singleton<GameDataSaveLoadManager>
{
    private string savePath;
    private GameData gameData;


    // 프로퍼티
    public GameData GameData => gameData;

    protected override void Awake()
    {
        base.Awake();
        // Application.persistentDataPath: 유니티에서 제공하는 특수한 폴더 경로를 반환하는 프로퍼티로 각 OS별로 유저 데이터를 저장하기에 안전한 전용 폴더 경로를 알려줌.
        savePath = Application.persistentDataPath + "/gamedata.json";

        gameData = LoadGame();
        if (gameData == null)
            gameData = new GameData();
    }

    // 게임 데이터 전체 저장 -> 전체 데이터 덮어쓰기
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("게임 데이터 저장 완료! : " + savePath);
    }

    // 게임 데이터 전체 로드
    public GameData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("저장된 캐릭터 데이터가 없습니다.");
            return null;
        }

        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);
        Debug.Log("게임 데이터 불러오기 완료!");
        return data;
    }

    public void SetSelectedCharacterSlotIndex(int index) => gameData.selectedCharacterSlotIndex = index;


    // 직업 선택 함수   
    // UI에서 JobData를 UI와 연결하고
    // 직업 선택하고 캐릭터 선택시 게임 CreateCharacter 함수와 연결

    public void CreateCharacter(CharacterData characterData, JobData selectedJob)
    {
        characterData.playerSaveData.jobData = selectedJob;
        characterData.playerSaveData.maxHealth = selectedJob.baseHP;
        // 나머지 데이터 넣기
    }
}

