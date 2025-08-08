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

        toolTip.SetActive(false);
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
            if (itemData is EquipmentItemData eq == false)
            {
                Debug.LogError("{TooltipUI} : itemData를 EquipmentItemData로 형변환을 할 수 없습니다!");
                return;
            }

            // UI 업데이트
            inventoryUI.SetWeaponSlot(eq, true);
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

            if (itemData is EquipmentItemData eq == false)
            {
                Debug.LogError("{TooltipUI} : itemData를 EquipmentItemData로 형변환을 할 수 없습니다!");
                return;
            }

            // UI 업데이트
            inventoryUI.SetWeaponSlot(eq, false);
            // 장비 해제
            // InventoryManager.Instance.UnEquipItem(itemData.ItemID, 플레이어 객체);
        }
        toolTip.SetActive(false);
    }


    public void Set(ItemData itemData, Vector2 screenPos)
    {
        this.itemData = itemData;

        itemImage.sprite = itemData.ItemSprite;
        itemName.text = itemData.ItemName;
        itemType.text = itemData.ItemType.ToString();
        itemText.text = itemData.Description;

        // 화면 위치에 툴팁 배치 (원하면 캔버스 좌표 변환 추가)
        // RectTransform rt = toolTip.GetComponent<RectTransform>();
        // if (rt != null)
        // {
        //     rt.position = screenPos;
        // }

        toolTip.SetActive(true);
    }

    public void Clear()
    {
        itemData = null;
        itemImage.sprite = null;
        itemName.text = "";
        itemType.text = "";
        itemText.text = "";
        toolTip.SetActive(false);
    }
}
