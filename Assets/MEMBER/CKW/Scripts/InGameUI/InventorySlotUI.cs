using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI countText;

    // InventoryUI에서 구독
    public Action<InventorySlotUI, Vector2> onClick;

    [Header("Visuals")]
    [SerializeField] private GameObject equippedOverlay; // 붉은 오버레이

    private ItemData itemData;
    private int count;
    private string uid; // ★ 슬롯 UID

    public ItemData ItemData => itemData;
    public int Count => count;
    public string Uid => uid;

    public void Set(ItemData data, int count, string uid)
    {
        this.itemData = data;
        this.count = count;
        this.uid = uid;

        icon.sprite = data != null ? data.ItemSprite : null;
        countText.text = (data != null && count > 1) ? count.ToString() : "";
        gameObject.SetActive(data != null);
        SetEquipped(false);
    }

    public void Clear()
    {
        itemData = null;
        count = 0;
        uid = null;
        icon.sprite = null;
        countText.text = "";
        gameObject.SetActive(false);
        SetEquipped(false);
    }

    public void SetEquipped(bool on)
    {
        if (equippedOverlay != null) equippedOverlay.SetActive(on);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this, eventData.position);
    }
}
