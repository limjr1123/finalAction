using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] Button equipButton;
    [SerializeField] Button liftButton;
    [SerializeField] GameObject toolTip;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemType;
    public TextMeshProUGUI itemText;

    private ItemData itemData;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseUI);
        if (equipButton != null)
            equipButton.onClick.AddListener(ItemEquip);
        if (liftButton != null)
            liftButton.onClick.AddListener(ItemLift);
    }


    void OnCloseUI()
    {
        toolTip.SetActive(false);
    }


    void ItemEquip()
    {
        if (itemData == null) return;
        if (itemData.ItemType == ItemType.Equipment)
        {
            // InventoryManager.Instance.EquipItem(itemData.ItemID, 플레이어 객체);
        }
        else if (itemData.ItemType == ItemType.Consumable)
        {

        }
        toolTip.SetActive(false);
    }

    void ItemLift()
    {
        if (itemData == null) return;
        if (itemData.ItemType == ItemType.Equipment)
        {
            // InventoryManager.Instance.UnEquipItem(itemData.ItemID, 플레이어 객체);
        }
        toolTip.SetActive(false);
    }

    public void Set(ItemData itemData)
    {
        this.itemData = itemData;
        itemImage.sprite = itemData.ItemSprite;
        itemName.text = itemData.ItemName;
        itemType.text = itemData.ItemType.ToString();
        itemText.text = itemData.Description;
        toolTip.SetActive(true);
    }

    public void Clear()
    {
        itemImage.sprite = null;
        itemName.text = null;
        itemType.text = null;
        itemText.text = null;
        toolTip.SetActive(false);
    }
}
