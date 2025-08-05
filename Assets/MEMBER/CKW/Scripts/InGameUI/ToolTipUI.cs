using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI; // 자기 부모 객체에서 가져올 수 있긴 함.
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
            EquipmentItemData equipmentItemData = itemData as EquipmentItemData;
            if (equipmentItemData == null)
            {
                Debug.LogError("{TooltipUI} : itemData를 EquipmentItemData로 형변환을 할 수 없습니다!");
                return;
            }

            // UI 업데이트
            inventoryUI.SetWeaponSlot(equipmentItemData);
            // 장비 장착
            // InventoryManager.Instance.EquipItem(itemData.ItemID, 플레이어 객체);
        }
        else if (itemData.ItemType == ItemType.Consumable)
        {

        }
        // 혹시나 기타 아이템도 장착/사용 기능이 있으면.
        toolTip.SetActive(false);
    }

    void ItemLift()
    {
        if (itemData == null) return;
        if (itemData.ItemType == ItemType.Equipment)
        {
            EquipmentItemData equipmentItemData = itemData as EquipmentItemData;
            if (equipmentItemData == null)
            {
                Debug.LogError("{TooltipUI} : itemData를 EquipmentItemData로 형변환을 할 수 없습니다!");
                return;
            }

            // UI 업데이트
            inventoryUI.SetWeaponSlot(equipmentItemData);
            // 장비 해제
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
