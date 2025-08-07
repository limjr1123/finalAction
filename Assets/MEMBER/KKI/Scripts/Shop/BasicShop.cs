using System.Collections.Generic;
using UnityEngine;

public class BasicShop : IShop
{

    private List<ShopItem> items;
    private ICurrencySystem currencySystem;
    private IInventory inventory;

    public BasicShop(ICurrencySystem currencySystem, IInventory inventory)
    {
        this.currencySystem = currencySystem;
        this.inventory = inventory;
        items = new List<ShopItem>();
    }

    public void DisplayItems()
    {
        // UI에 아이템 표시
    }

    public void Purchase(string itemID)
    {
        ShopItem item = items.Find(i => i.ID == itemID);
        if (currencySystem.TrySpend(item.Price))
        {
            inventory.AddItem(itemID);
        }
    }

    public void AddItem(ShopItem item)
    {
        items.Add(item);
    }
}