using System.Collections.Generic;
using System.IO;
using GameSave;
using UnityEngine;

namespace GameSave
{
    [System.Serializable]
    public class GameData
    {
        public int selectedCharacterSlot; // 현재 선택된 캐릭터 슬롯
        public List<CharacterData> characters = new();
        //재화, 설정, 기타 데이터 추가
    }

    [System.Serializable]
    public class CharacterData
    {
        public string characterName;
        public int level;
        public int hp;
        public int exp;
        public InventorySaveData inventory = new();
        // 퀘스트 
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
}

public class GameDataSaveLoadManager : Singleton<GameDataSaveLoadManager>
{
    private string savePath;

    protected override void Awake()
    {
        base.Awake();
        // Application.persistentDataPath: 유니티에서 제공하는 특수한 폴더 경로를 반환하는 프로퍼티로 각 OS별로 유저 데이터를 저장하기에 안전한 전용 폴더 경로를 알려줌.
        savePath = Application.persistentDataPath + "/gamedata.json";
    }


    // 캐릭터 저장
    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("게임 데이터 저장 완료! : " + savePath);
    }


    public GameData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("저장된 캐릭터 데이터가 없습니다.");
            return null;
        }

        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);
        Debug.Log("캐릭터 불러오기 완료!");
        return data;
    }
}

