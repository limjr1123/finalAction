using System.Collections.Generic;
using UnityEngine;

public class ShopInstaller : MonoBehaviour
{
    public static BasicShop ShopInstance;

    [Header("상점에 진열할 아이템들")]
    public List<ItemData> shopItems;

    void Awake()
    {
        ICurrencySystem currency = FindAnyObjectByType<GoldSystem>();
        IInventory inventory = InventoryManager.Instance;
        ShopInstance = new BasicShop(currency, inventory);

        foreach (var item in shopItems)
            ShopInstance.AddItem(item);
    }
}
