using UnityEngine;
using GameSave;

public class GoldSystem : Singleton<GoldSystem>, ICurrencySystem
{
    private CharacterData character;

    protected override void Awake()
    {
        base.Awake();

        var mgr = GameDataSaveLoadManager.Instance;
        int idx = mgr.GetSelectedCharacterSlotIndex();
        character = mgr.GetCharacterData(idx);

        if (character == null)
        {
            Debug.LogError("선택된 캐릭터 데이터가 없습니다!");
        }
    }

    public bool TrySpend(int amount)
    {
        if (character == null) return false;
        if (character.gold >= amount)
        {
            character.gold -= amount;
            GameManager.Instance.SaveGame();
            return true;
        }
        return false;
    }

    public int GetBalance() => character?.gold ?? 0;

    public void AddCurrency(int amount)
    {
        if (character == null) return;
        character.gold += amount;
        GameManager.Instance.SaveGame();
    }
}
