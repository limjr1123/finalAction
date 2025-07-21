using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> allItems = new();

    private Dictionary<string, ItemData> itemMap;

    public void Initialize()
    {
        itemMap = allItems.ToDictionary(allItems => allItems.ItemID);
    }

    public ItemData GetItemData(string itemID)
    {
        if (itemMap == null)
        {
            Initialize();
        }

        return itemMap.TryGetValue(itemID, out var data) ? data : null;
    }

    public bool Contains(string itemID) => allItems.Any(i => i.ItemID == itemID);
}
