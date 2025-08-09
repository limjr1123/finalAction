using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI countText;
    public ToolTipUI toolTipUI;

    // 클릭을 InventoryUI로 전달하기 위한 콜백
    public Action<InventorySlotUI, Vector2> onClick;

    private ItemData itemData;
    private int count;

    public ItemData ItemData => itemData;
    public int Count => count;

    public void Set(ItemData data, int count)
    {
        itemData = data;
        this.count = count;
        icon.sprite = data != null ? data.ItemSprite : null;
        countText.text = (data != null && count > 1) ? count.ToString() : "";
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
        onClick?.Invoke(this, eventData.position);
    }
}
