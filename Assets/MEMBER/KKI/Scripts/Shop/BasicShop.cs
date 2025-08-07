using System.Collections.Generic;

public class BasicShop : IShop
{

    private List<ItemData> items;
    private ICurrencySystem currencySystem;
    private IInventory inventory;

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
        if (item != null && currencySystem.TrySpend(item.Gold)) // 가격: item.Gold
        {
            inventory.AddItem(itemID);
            return true;
        }
        return false;
    }
}