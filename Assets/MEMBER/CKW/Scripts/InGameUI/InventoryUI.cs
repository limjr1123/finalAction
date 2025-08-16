using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : BaseUI
{
    [Header("Slot Areas")]
    [SerializeField] Transform EquipmentSlotArea;
    [SerializeField] Transform ConsumableSlotArea;
    [SerializeField] Transform EtcSlotArea;

    [Header("Inventory Toggles")]
    [SerializeField] Toggle EquipmentTabToggle;
    [SerializeField] Toggle ConsumableTabToggle;
    [SerializeField] Toggle EtcTabToggle;
    [SerializeField] ToggleGroup TabToggleGroup;

    [Header("Inventory Buttons")]
    [SerializeField] Button closeButton;
    [SerializeField] Button sellButton;

    [Header("Tooltip")]
    [SerializeField] ToolTipUI toolTipUI;

    [Header("Equipment Slot")]
    [SerializeField] Image weaponSlotImage;
    [SerializeField] Image bodySlotImage;
    [SerializeField] Image accessoryImage;
    [SerializeField] Sprite originWeaponSlotImage;
    [SerializeField] Sprite originBodySlotImage;
    [SerializeField] Sprite originAccessoryImage;

    [Header("Gold")]
    [SerializeField] TextMeshProUGUI goldText;

    private InventorySlotUI[] equipmentSlots;
    private InventorySlotUI[] consumableSlots;
    private InventorySlotUI[] etcSlots;

    private InventorySlotUI _selectedSlot;
    private int _selectedIndex = -1;
    private string _selectedUid;
    public int SelectedSlotIndex => _selectedIndex;
    public string SelectedSlotUid => _selectedUid;

    private enum Tab { Equip, Consumable, Etc }
    private Tab _currentTab = Tab.Equip;

    void Awake()
    {
        EquipmentSlotArea.gameObject.SetActive(false);
        ConsumableSlotArea.gameObject.SetActive(false);
        EtcSlotArea.gameObject.SetActive(false);

        equipmentSlots = EquipmentSlotArea.GetComponentsInChildren<InventorySlotUI>(true);
        consumableSlots = ConsumableSlotArea.GetComponentsInChildren<InventorySlotUI>(true);
        etcSlots = EtcSlotArea.GetComponentsInChildren<InventorySlotUI>(true);

        SubscribeSlotClicks(equipmentSlots);
        SubscribeSlotClicks(consumableSlots);
        SubscribeSlotClicks(etcSlots);

        EquipmentTabToggle.group = TabToggleGroup;
        ConsumableTabToggle.group = TabToggleGroup;
        EtcTabToggle.group = TabToggleGroup;
    }

    void Start()
    {
        if (closeButton != null) closeButton.onClick.AddListener(CloseInventoryUI);
        if (sellButton != null) sellButton.onClick.AddListener(SellItem);

        EquipmentTabToggle.onValueChanged.AddListener(OnEquipmentTabToggle);
        ConsumableTabToggle.onValueChanged.AddListener(OnConsumableTabToggle);
        EtcTabToggle.onValueChanged.AddListener(OnEtcTabToggle);

        if (EquipmentTabToggle.isOn) ShowEquipmentTab();
        else if (ConsumableTabToggle.isOn) ShowConsumableTab();
        else if (EtcTabToggle.isOn) ShowEtcTab();

        UpdateGoldUI();
    }

    private void CloseInventoryUI()
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Select);
        SelectSlot(null, -1, null);
        CloseUI();
    }

    // ===== 판매 =====
    private void SellItem()
    {
        if (_selectedSlot == null || _selectedSlot.ItemData == null)
        {
            Debug.LogWarning("[InventoryUI] 판매할 아이템이 선택되지 않았습니다.");
            return;
        }

        var item = _selectedSlot.ItemData;
        int count = _selectedSlot.Count;
        string uid = _selectedUid;

        if (count <= 0) { Debug.LogWarning("[InventoryUI] 선택된 슬롯 수량 0"); return; }

        int unitPrice = item.Gold;
        int totalPrice = unitPrice * count;

        // 장착 슬롯인지 미리 체크
        EquipmentItemData eq = item as EquipmentItemData;
        bool sellingEquipped = (eq != null) && EquipmentState.IsEquipped(eq.EquipType, uid);

        InventoryManager.Instance.RemoveItem(item.ItemID, count);

        // 장착 중이던 슬롯을 팔았다면 즉시 장착 해제(아이콘 원복)
        if (sellingEquipped && eq != null)
        {
            EquipmentState.Clear(eq.EquipType);
            SetWeaponSlot(eq, false);
        }

        var goldSystem = FindAnyObjectByType<GoldSystem>();
        if (goldSystem != null) goldSystem.AddCurrency(totalPrice);

        RefreshCurrentTab();
        SelectSlot(null, -1, null);
        UpdateGoldUI();

        Debug.Log($"[InventoryUI] {item.ItemName} {count}개 판매 → {totalPrice} 골드");
    }

    // ===== 골드 UI =====
    private void UpdateGoldUI()
    {
        if (goldText == null) return;
        var goldSystem = FindAnyObjectByType<GoldSystem>();
        goldText.text = goldSystem != null ? goldSystem.GetBalance().ToString() : "0";
    }

    // ===== 슬롯 클릭/선택 =====
    private void SubscribeSlotClicks(IEnumerable<InventorySlotUI> slots)
    {
        foreach (var slot in slots) slot.onClick = OnSlotClicked;
    }

    private void OnSlotClicked(InventorySlotUI slot, Vector2 screenPos)
    {
        if (slot == null || slot.ItemData == null)
        {
            SelectSlot(null, -1, null);
            return;
        }

        int index = -1;
        if (EquipmentSlotArea.gameObject.activeSelf)
        {
            for (int i = 0; i < equipmentSlots.Length; i++) if (equipmentSlots[i] == slot) { index = i; break; }
        }
        else if (ConsumableSlotArea.gameObject.activeSelf)
        {
            for (int i = 0; i < consumableSlots.Length; i++) if (consumableSlots[i] == slot) { index = i; break; }
        }
        else if (EtcSlotArea.gameObject.activeSelf)
        {
            for (int i = 0; i < etcSlots.Length; i++) if (etcSlots[i] == slot) { index = i; break; }
        }

        SelectSlot(slot, index, slot.Uid);

        if (toolTipUI != null)
        {
            toolTipUI.Set(slot.ItemData, screenPos);
        }
    }

    private void SelectSlot(InventorySlotUI slot, int index, string uid)
    {
        _selectedSlot = slot;
        _selectedIndex = index;
        _selectedUid = uid;

        if (_selectedSlot == null && toolTipUI != null)
            toolTipUI.Clear();
    }

    // ===== 탭 전환/표시 =====
    void OnEquipmentTabToggle(bool isOn)
    {
        if (isOn) { _currentTab = Tab.Equip; ShowEquipmentTab(); }
        else EquipmentSlotArea.gameObject.SetActive(false);
    }
    void OnConsumableTabToggle(bool isOn)
    {
        if (isOn) { _currentTab = Tab.Consumable; ShowConsumableTab(); }
        else ConsumableSlotArea.gameObject.SetActive(false);
    }
    void OnEtcTabToggle(bool isOn)
    {
        if (isOn) { _currentTab = Tab.Etc; ShowEtcTab(); }
        else EtcSlotArea.gameObject.SetActive(false);
    }

    void ShowEquipmentTab()
    {
        SetActiveAreas(EquipmentSlotArea.transform);
        var list = InventoryManager.Instance.GetAllEquipmentInventory;

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (i < list.Count)
            {
                var slot = list[i];
                equipmentSlots[i].Set(slot.data, slot.count, slot.uid);

                if (slot.data is EquipmentItemData eq)
                    equipmentSlots[i].SetEquipped(EquipmentState.IsEquipped(eq.EquipType, slot.uid));
                else
                    equipmentSlots[i].SetEquipped(false);
            }
            else
            {
                equipmentSlots[i].Clear();
            }
        }
    }

    void ShowConsumableTab()
    {
        SetActiveAreas(ConsumableSlotArea.transform);
        var list = InventoryManager.Instance.GetAllConsumableInventory;

        for (int i = 0; i < consumableSlots.Length; i++)
        {
            if (i < list.Count)
            {
                var slot = list[i];
                consumableSlots[i].Set(slot.data, slot.count, slot.uid);
            }
            else
            {
                consumableSlots[i].Clear();
            }
        }
    }

    void ShowEtcTab()
    {
        SetActiveAreas(EtcSlotArea.transform);
        var list = InventoryManager.Instance.GetAllEtcInventory;

        for (int i = 0; i < etcSlots.Length; i++)
        {
            if (i < list.Count)
            {
                var slot = list[i];
                etcSlots[i].Set(slot.data, slot.count, slot.uid);
            }
            else
            {
                etcSlots[i].Clear();
            }
        }
    }

    void SetActiveAreas(Transform activeArea)
    {
        EquipmentSlotArea.gameObject.SetActive(activeArea == EquipmentSlotArea);
        ConsumableSlotArea.gameObject.SetActive(activeArea == ConsumableSlotArea);
        EtcSlotArea.gameObject.SetActive(activeArea == EtcSlotArea);
    }

    // ===== 장착/해제 (툴팁에서 호출) =====
    public void EquipSelected(EquipmentItemData eq, bool equipOn)
    {
        if (eq == null || _currentTab != Tab.Equip || string.IsNullOrEmpty(_selectedUid)) return;

        if (equipOn)
            EquipmentState.SetEquipped(eq.EquipType, _selectedUid);
        else
            EquipmentState.Clear(eq.EquipType);

        SetWeaponSlot(eq, equipOn); // 장비창 이미지 반영
        ShowEquipmentTab();         // 리스트 오버레이 반영
    }

    // 장비 슬롯 UI 갱신
    public void SetWeaponSlot(EquipmentItemData equipmentItemData, bool flag)
    {
        switch (equipmentItemData.EquipType)
        {
            case EquipType.Weapon:
                weaponSlotImage.sprite = flag ? equipmentItemData.ItemSprite : originWeaponSlotImage; break;
            case EquipType.Body:
                bodySlotImage.sprite = flag ? equipmentItemData.ItemSprite : originBodySlotImage; break;
            case EquipType.Accessory:
                accessoryImage.sprite = flag ? equipmentItemData.ItemSprite : originAccessoryImage; break;
        }
    }

    private void RefreshCurrentTab()
    {
        if (EquipmentTabToggle.isOn) ShowEquipmentTab();
        else if (ConsumableTabToggle.isOn) ShowConsumableTab();
        else if (EtcTabToggle.isOn) ShowEtcTab();
    }
}
