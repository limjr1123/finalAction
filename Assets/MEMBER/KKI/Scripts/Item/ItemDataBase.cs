using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class ItemDatas
{
    public string desription;
    public ItemData itemData;
}

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemDatas> items = new();
    private Dictionary<string, ItemDatas> itemDict;

    public void Initialize()
    {
        itemDict = items.ToDictionary(items => items.itemData.ItemID);
    }

    public ItemData GetItemData(string itemID)
    {
        if (itemDict == null)
        {
            Initialize();
        }

        return itemDict.TryGetValue(itemID, out var item) ? item.itemData : null;
    }

    public bool Contains(string itemID) => items.Any(i => i.itemData.ItemID == itemID);
}
