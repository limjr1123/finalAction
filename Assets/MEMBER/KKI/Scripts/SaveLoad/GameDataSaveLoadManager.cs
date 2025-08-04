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
        public InventorySaveData inventorySaveData = new();
        public QuestSaveData questSaveData = new();
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<InventorySlotSaveData> equipSlotSaveData = new();
        public List<InventorySlotSaveData> consumableSlotSaveData = new();
        public List<InventorySlotSaveData> etcSlotSaveData = new();
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
        public List<string> activeQuests = new();
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

    private CharacterFactory characterFactory = new CharacterFactory();


    // 프로퍼티
    public GameData GameData => gameData;

    protected override void Awake()
    {
        base.Awake();
        // Application.persistentDataPath: 유니티에서 제공하는 특수한 폴더 경로를 반환하는 프로퍼티로 각 OS별로 유저 데이터를 저장하기에 안전한 전용 폴더 경로를 알려줌.
        savePath = Application.persistentDataPath + "/gamedata.json";

        gameData = LoadGameDataFromJason();
        if (gameData == null)
            gameData = new GameData();
    }

    // 게임 데이터 전체 저장 -> 전체 데이터 덮어쓰기
    public void SaveGameDataToJason()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("게임 데이터 저장 완료! : " + savePath);
    }


    // 특정 인덱스의 캐릭터 데이터 가져오기
    public CharacterData GetCharacterData(int index)
    {
        if (gameData.characters.Count == 0)
        {
            Debug.LogWarning("저장된 캐릭터가 없습니다!");
            return null;
        }

        if (index < 0 || index >= gameData.characters.Count)
        {
            Debug.LogWarning($"잘못된 캐릭터 인덱스: {index}");
            return null;
        }

        return gameData.characters[index];
    }
    // 게임 데이터 전체 로드
    public GameData LoadGameDataFromJason()
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

    // 선택된 캐릭터 슬롯 인덱스 가져오기 (누락된 메서드 추가)
    public int GetSelectedCharacterSlotIndex()
    {
        return gameData.selectedCharacterSlotIndex;
    }

    // 직업 선택 함수   
    // UI에서 JobData를 UI와 연결하고
    // 직업 선택하고 캐릭터 선택시 게임 CreateCharacter 함수와 연결
    public void CreateCharacter(string characterName, JobData selectedJob)
    {
        // 1. 새 캐릭터 데이터 생성
        CharacterData newChar = characterFactory.CreateCharacter(characterName, selectedJob);

        // 2. GameData에 추가
        gameData.characters.Add(newChar);
        gameData.selectedCharacterSlotIndex = gameData.characters.Count - 1;

        // 3. 저장
        SaveGameDataToJason();
        Debug.Log($"{characterName} 캐릭터 생성 완료!");

        // 4. UI 갱신 등 추가 작업
    }

}

