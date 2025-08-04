using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : BaseUI
{

    [Header("Inventory Type")]
    [SerializeField] Toggle EquipmentTabToggle;
    [SerializeField] Toggle ConsumableTabToggle;
    [SerializeField] Toggle EtcTabToggle;


    [Header("Inventory Buttons")]
    [SerializeField] Button closeButton;
    [SerializeField] Button sellButton;


    [SerializeField] GameObject EquipmentSlotArea;
    [SerializeField] GameObject ConsumableSlotArea;
    [SerializeField] GameObject EtcSlotArea;

    [Header("Prefabs & Tooltip")]
    [SerializeField] ToolTipUI toolTipUI;

    private InventorySlotUI[] equipmentSlots;
    private InventorySlotUI[] consumableSlots;
    private InventorySlotUI[] etcSlots;

    void Awake()
    {
        // 자식에서 자동으로 가져옴
        equipmentSlots = EquipmentSlotArea.GetComponentsInChildren<InventorySlotUI>(true);
        consumableSlots = ConsumableSlotArea.GetComponentsInChildren<InventorySlotUI>(true);
        etcSlots = EtcSlotArea.GetComponentsInChildren<InventorySlotUI>(true);

        // 슬롯에 툴팁UI 연결 
        foreach (var slot in equipmentSlots) slot.toolTipUI = toolTipUI;
        foreach (var slot in consumableSlots) slot.toolTipUI = toolTipUI;
        foreach (var slot in etcSlots) slot.toolTipUI = toolTipUI;
    }

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventoryUI);
        if (sellButton != null)
            sellButton.onClick.AddListener(SellItem);


        if (EquipmentTabToggle != null)
            EquipmentTabToggle.onValueChanged.AddListener((ison) => OnEquipmentTabToggle(ison));
        if (ConsumableTabToggle != null)
            ConsumableTabToggle.onValueChanged.AddListener((ison) => OnConsumableTabToggle(ison));
        if (EtcTabToggle != null)
            EtcTabToggle.onValueChanged.AddListener((ison) => OnEtcTabToggle(ison));

        // 기본 탭 활성화
        ShowEquipmentTab();
    }


    private void CloseInventoryUI()
    {
        CloseUI();
    }


    private void SellItem()
    {
        // 게임 머니 얻기
    }

    private void OnEquipmentTabToggle(bool isOn)
    {
        if (isOn) ShowEquipmentTab();
    }

    private void OnConsumableTabToggle(bool isOn)
    {
        if (isOn) ShowConsumableTab();
    }

    private void OnEtcTabToggle(bool isOn)
    {
        if (isOn) ShowEtcTab();
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
}

