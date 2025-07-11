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
        // 캐릭터 데이터 -> 나중에 구조체 및 클래스로 대체
        public string characterName;
        public int level;
        public int hp;
        public int exp;
        public InventorySaveData inventoryData = new();
        public QuestSaveData questData = new();
        // 추가.
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

    // 더미 데이터 생성 함수 추가 
    public void CreateDummyData()
    {
        var data = new GameSave.GameData();
        data.selectedCharacterSlotIndex = 0;

        // 캐릭터 1
        data.characters.Add(new GameSave.CharacterData
        {
            characterName = "용사A",
            level = 10,
            hp = 120,
            exp = 350,
            inventoryData = new GameSave.InventorySaveData
            {
                items = new List<GameSave.InventorySlotSaveData>
                {
                    new GameSave.InventorySlotSaveData { itemID = "potion", count = 3 },
                    new GameSave.InventorySlotSaveData { itemID = "sword", count = 1 }
                }
            },
            questData = new GameSave.QuestSaveData
            {
                currentQuests = new List<string> { "슬라임 10마리 처치" },
                completedQuests = new List<string> { "튜토리얼 완료" }
            }
        });

        // 캐릭터 2
        data.characters.Add(new GameSave.CharacterData
        {
            characterName = "마법사B",
            level = 7,
            hp = 80,
            exp = 140,
            inventoryData = new GameSave.InventorySaveData
            {
                items = new List<GameSave.InventorySlotSaveData>
                {
                    new GameSave.InventorySlotSaveData { itemID = "mana_potion", count = 5 }
                }
            },
            questData = new GameSave.QuestSaveData()
        });

        // 유저 설정
        data.userSettings = new GameSave.UserSettings
        {
            bgmVolume = 0.7f,
            sfxVolume = 0.8f,
            screenResolution = 1080,
            isFullScreen = true
        };

        this.gameData = data;
        SaveGame();
        Debug.Log("가짜(더미) 테스트 데이터 생성/저장 완료!");
    }
}

