using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] Button closeButton;
    [SerializeField] Button equipButton; // 장착
    [SerializeField] Button liftButton;  // 해제
    [SerializeField] GameObject toolTip;

    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemType;
    public TextMeshProUGUI itemText;

    private ItemData itemData;

    void Start()
    {
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseUI);
        if (equipButton != null) equipButton.onClick.AddListener(ItemEquip);
        if (liftButton != null) liftButton.onClick.AddListener(ItemLift);
        toolTip.SetActive(false);
    }

    void OnCloseUI() => toolTip.SetActive(false);

    void ItemEquip()
    {
        if (itemData is EquipmentItemData eq) inventoryUI.EquipSelected(eq, true);
        toolTip.SetActive(false);
    }

    void ItemLift()
    {
        if (itemData is EquipmentItemData eq) inventoryUI.EquipSelected(eq, false);
        toolTip.SetActive(false);
    }

    public void Set(ItemData itemData, Vector2 _)
    {
        this.itemData = itemData;

        itemImage.sprite = itemData.ItemSprite;
        itemName.text = itemData.ItemName;
        itemType.text = itemData.ItemType.ToString();
        itemText.text = itemData.Description;

        // 장착 상태에 따라 버튼 토글 (UID 기준)
        bool showEquip = false, showLift = false;
        if (itemData is EquipmentItemData eq)
        {
            string uid = inventoryUI.SelectedSlotUid;
            bool equipped = !string.IsNullOrEmpty(uid) && EquipmentState.IsEquipped(eq.EquipType, uid);
            showEquip = !equipped;
            showLift = equipped;
        }
        if (equipButton != null) equipButton.gameObject.SetActive(showEquip);
        if (liftButton != null) liftButton.gameObject.SetActive(showLift);

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
