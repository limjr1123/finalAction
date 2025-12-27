using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInstaller : MonoBehaviour
{
    public static BasicShop ShopInstance;

    [Header("상점에 진열할 아이템들")]
    public List<ItemData> shopItems;

    // ⭐ Awake → Start 로 변경
    void Start()
    {
        StartCoroutine(Initialized());
    }

    public IEnumerator Initialized()
    {
        yield return new WaitForSeconds(1f); // MonoBehaviour의 Awake가 끝날 때까지 대기
        // 이 시점엔 보통 다른 싱글톤들의 Awake가 끝난 뒤라 null 가능성↓
        ICurrencySystem currency = FindAnyObjectByType<GoldSystem>();
        IInventory inventory = InventoryManager.Instance;

        if (currency == null) { Debug.LogError("[ShopInstaller] GoldSystem 찾지 못함"); yield return null; }
        if (inventory == null) { Debug.LogError("[ShopInstaller] InventoryManager 인스턴스 없음"); yield return null; }

        ShopInstance = new BasicShop(currency, inventory);

        if (shopItems != null)
        {
            foreach (var item in shopItems)
            {
                if (item == null) { Debug.LogWarning("[ShopInstaller] null ItemData 스킵"); continue; }
                ShopInstance.AddItem(item);
            }
        }
    }
}