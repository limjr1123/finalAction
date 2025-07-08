using UnityEngine;
using GameSave;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();

        // 서비스 등록
        ServiceLocator.Register(this);
        ServiceLocator.Register(SceneLoader.Instance);
        ServiceLocator.Register(GameDataSaveLoadManager.Instance);
        ServiceLocator.Register(InventoryManager.Instance);
    }

    void Start()
    {

    }

    public void GameStart()
    {
        var gameData = ServiceLocator.Get<GameDataSaveLoadManager>().LoadGame();
        if (gameData == null) return;

        int slot = gameData.selectedCharacterSlot;
        if (gameData.characters.Count > slot)
        {
            var charData = gameData.characters[slot];
            ServiceLocator.Get<InventoryManager>().LoadInventory(charData.inventory);
        }
    }
}