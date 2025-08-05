using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private InventorySlotUI[] equipmentSlots;
    private InventorySlotUI[] consumableSlots;
    private InventorySlotUI[] etcSlots;

    void Awake()
    {
        EquipmentSlotArea.gameObject.SetActive(false);
        ConsumableSlotArea.gameObject.SetActive(false);
        EtcSlotArea.gameObject.SetActive(false);

        // 슬롯 자동 할당 (비활성 슬롯도 포함)
        equipmentSlots = EquipmentSlotArea.GetComponentsInChildren<InventorySlotUI>(true);
        consumableSlots = ConsumableSlotArea.GetComponentsInChildren<InventorySlotUI>(true);
        etcSlots = EtcSlotArea.GetComponentsInChildren<InventorySlotUI>(true);

        // 툴팁UI 할당
        foreach (var slot in equipmentSlots) slot.toolTipUI = toolTipUI;
        foreach (var slot in consumableSlots) slot.toolTipUI = toolTipUI;
        foreach (var slot in etcSlots) slot.toolTipUI = toolTipUI;

        // ToggleGroup 연결
        EquipmentTabToggle.group = TabToggleGroup;
        ConsumableTabToggle.group = TabToggleGroup;
        EtcTabToggle.group = TabToggleGroup;
    }

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventoryUI);
        if (sellButton != null)
            sellButton.onClick.AddListener(SellItem);

        EquipmentTabToggle.onValueChanged.AddListener(OnEquipmentTabToggle);
        ConsumableTabToggle.onValueChanged.AddListener(OnConsumableTabToggle);
        EtcTabToggle.onValueChanged.AddListener(OnEtcTabToggle);

        // 시작 시 첫 탭 활성화 (Inspector에서 isOn 조정 가능)
        if (EquipmentTabToggle.isOn) ShowEquipmentTab();
        else if (ConsumableTabToggle.isOn) ShowConsumableTab();
        else if (EtcTabToggle.isOn) ShowEtcTab();
    }


    private void CloseInventoryUI()
    {
        CloseUI();
    }


    private void SellItem()
    {
        // 게임 머니 얻기
    }

    #region 토글에 따라 인벤토리 슬롯 보여주기

    void OnEquipmentTabToggle(bool isOn)
    {
        if (isOn) ShowEquipmentTab();
        else EquipmentSlotArea.gameObject.SetActive(false);
    }
    void OnConsumableTabToggle(bool isOn)
    {
        if (isOn) ShowConsumableTab();
        else ConsumableSlotArea.gameObject.SetActive(false);
    }
    void OnEtcTabToggle(bool isOn)
    {
        if (isOn) ShowEtcTab();
        else EtcSlotArea.gameObject.SetActive(false);
    }


    void ShowEquipmentTab()
    {
        SetActiveAreas(EquipmentSlotArea.transform);
        var list = InventoryManager.Instance.GetAllEquipmentInventory;
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (i < list.Count)
                equipmentSlots[i].Set(list[i].data, list[i].count);
            else
                equipmentSlots[i].Clear();
        }
    }

    void ShowConsumableTab()
    {
        SetActiveAreas(ConsumableSlotArea.transform);
        var list = InventoryManager.Instance.GetAllConsumableInventory;
        for (int i = 0; i < consumableSlots.Length; i++)
        {
            if (i < list.Count)
                consumableSlots[i].Set(list[i].data, list[i].count);
            else
                consumableSlots[i].Clear();
        }
    }

    void ShowEtcTab()
    {
        SetActiveAreas(EtcSlotArea.transform);
        var list = InventoryManager.Instance.GetAllEtcInventory;
        for (int i = 0; i < etcSlots.Length; i++)
        {
            if (i < list.Count)
                etcSlots[i].Set(list[i].data, list[i].count);
            else
                etcSlots[i].Clear();
        }
    }

    void SetActiveAreas(Transform activeArea)
    {
        EquipmentSlotArea.gameObject.SetActive(activeArea == EquipmentSlotArea);
        ConsumableSlotArea.gameObject.SetActive(activeArea == ConsumableSlotArea);
        EtcSlotArea.gameObject.SetActive(activeArea == EtcSlotArea);
    }

    #endregion



    // 장비 슬롯 UI 갱신 함수
    public void SetWeaponSlot(EquipmentItemData equipmentItemData)
    {
        Sprite sprite;
        bool isNull;

        if (equipmentItemData == null)
        {
            sprite = null;
            isNull = false;
        }
        else
        {
            sprite = equipmentItemData.ItemSprite;
            isNull = true;
        }

        switch (equipmentItemData.EquipType)
        {
            case EquipType.Weapon:
                {
                    weaponSlotImage.sprite = sprite;
                    weaponSlotImage.enabled = isNull;
                    break;
                }
            case EquipType.Body:
                {
                    bodySlotImage.sprite = sprite;
                    bodySlotImage.enabled = isNull;
                    break;
                }
            case EquipType.Accessory:
                {
                    accessoryImage.sprite = sprite;
                    accessoryImage.enabled = isNull;
                    break;
                }
        }

    }
}

