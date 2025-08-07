using System.Collections.Generic;

public interface IShop
{
    bool Purchase(string itemID);
    List<ItemData> GetAllItems();
    void AddItem(ItemData item);
}