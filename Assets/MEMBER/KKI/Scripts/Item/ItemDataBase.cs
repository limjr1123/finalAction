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
    [SerializeField] private List<ItemDatas> Items = new();

    private Dictionary<string, ItemDatas> itemMap;

    public void Initialize()
    {
        itemMap = Items.ToDictionary(Items => Items.itemData.ItemID);
    }

    public ItemData GetItemData(string itemID)
    {
        if (itemMap == null)
        {
            Initialize();
        }

        return itemMap.TryGetValue(itemID, out var data) ? data.itemData : null;
    }

    public bool Contains(string itemID) => Items.Any(i => i.itemData.ItemID == itemID);
}
