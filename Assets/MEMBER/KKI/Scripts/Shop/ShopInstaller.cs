using UnityEngine;

public class ShopInstaller : MonoBehaviour
{
    void Start()
    {
        ICurrencySystem currency = FindAnyObjectByType<GoldSystem>();
        IInventory inventory = InventoryManager.Instance;

        BasicShop shop = new BasicShop(currency, inventory);
        shop.AddItem(new ShopItem { ID = "sword", Name = "Sword", Price = 100 });

        shop.Purchase("sword");
    }
}