using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasicShop : IShop
{

    public List<ItemData> items;
    public ICurrencySystem currencySystem;
    public IInventory inventory;

    public BasicShop(ICurrencySystem currencySystem, IInventory inventory)
    {
        this.currencySystem = currencySystem;
        this.inventory = inventory;
        items = new List<ItemData>();
    }

    public List<ItemData> GetAllItems() => items;

    public void AddItem(ItemData item) => items.Add(item);
    public bool Purchase(string itemID)
    {
        ItemData item = items.Find(i => i.ItemID == itemID);
        Debug.Log(item.Gold);
        if (item != null && currencySystem.TrySpend(item.Gold)) // 가격: item.Gold
        {
            inventory.AddItem(itemID);
            return true;
        }
        return false;
    }
}