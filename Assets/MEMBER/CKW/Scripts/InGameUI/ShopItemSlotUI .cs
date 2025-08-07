using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    private string itemID;
    private ShopUI shopUI;

    public void Set(ItemData item, ShopUI parent)
    {
        itemID = item.ItemID;
        shopUI = parent;

        icon.sprite = item.ItemSprite;
        icon.enabled = true;
        nameText.text = item.ItemName;
        priceText.text = $"{item.Gold} Gold";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuy);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    void OnBuy()
    {
        shopUI.TryPurchase(itemID);
    }
}
