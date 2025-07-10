using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();

        // 서비스 등록
        // ServiceLocator.Register
    }

    void Start()
    {

    }

    /// <summary>
    /// 캐릭터 관련 나머지 데이터 로드할 때 사용 (인벤토리, 퀘스트 등등)
    /// </summary>
    public void GameStart()
    {
        var gameData = GameDataSaveLoadManager.Instance.GameData;
        if (gameData == null) return;

        int slotIndex = gameData.selectedCharacterSlotIndex; // 현재 선택된 캐릭터
        if (gameData.characters.Count > slotIndex && slotIndex >= 0)
        {
            var charData = gameData.characters[slotIndex];
            InventoryManager.Instance.LoadInventory(charData.inventoryData);
            // 퀘스트도 연결
        }
    }
}