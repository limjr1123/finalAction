using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI countText;
    public ToolTipUI toolTipUI;

    private ItemData itemData;
    private int count;

    public void Set(ItemData data, int count)
    {
        itemData = data;
        this.count = count;
        icon.sprite = data.ItemSprite;
        countText.text = count > 1 ? count.ToString() : "";
        gameObject.SetActive(true);
    }

    public void Clear()
    {
        itemData = null;
        icon.sprite = null;
        countText.text = "";
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemData != null && toolTipUI != null)
        {
            toolTipUI.Set(itemData);
            toolTipUI.transform.position = eventData.position; // 터치/클릭 위치에 표시
        }
    }
}
