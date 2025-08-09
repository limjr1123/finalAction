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

    [Header("Gold")]
    [SerializeField] TextMeshProUGUI goldText;

    private InventorySlotUI[] equipmentSlots;
    private InventorySlotUI[] consumableSlots;
    private InventorySlotUI[] etcSlots;

    // 현재 선택된 슬롯(툴팁과 판매의 기준)
    private InventorySlotUI _selectedSlot;

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
        SubscribeSlotClicks(equipmentSlots);
        SubscribeSlotClicks(consumableSlots);
        SubscribeSlotClicks(etcSlots);

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

        UpdateGoldUI();
    }


    private void CloseInventoryUI()
    {
        SelectSlot(null);
        CloseUI();
    }

    private void SellItem()
    {
        // 1) 선택 슬롯이 있는지 확인
        if (_selectedSlot == null || _selectedSlot.ItemData == null)
        {
            Debug.LogWarning("[InventoryUI] 판매할 아이템이 선택되지 않았습니다.");
            return;
        }

        var item = _selectedSlot.ItemData;
        int count = _selectedSlot.Count;
        if (count <= 0)
        {
            Debug.LogWarning("[InventoryUI] 선택된 슬롯의 수량이 0입니다.");
            return;
        }

        // 2) 판매 가격 계산 (개당 가격 × 개수)
        int unitPrice = item.Gold; // <-- 필요 시 확장 포인트
        int totalPrice = unitPrice * count;

        // 3) 인벤토리에서 제거
        InventoryManager.Instance.RemoveItem(item.ItemID, count);

        // 4) 골드 추가 
        var goldSystem = FindAnyObjectByType<GoldSystem>();
        if (goldSystem != null)
        {
            goldSystem.AddCurrency(totalPrice);
        }
        else
        {
            Debug.LogError("[InventoryUI] GoldSystem을 찾을 수 없어 골드를 지급하지 못했습니다.");
        }

        // 5) UI 갱신 + 선택 해제 + 툴팁 닫기
        RefreshCurrentTab();
        SelectSlot(null);
        UpdateGoldUI();

        Debug.Log($"[InventoryUI] {item.ItemName} {count}개 판매 → {totalPrice} 골드 획득");
    }

    // 현재 켜져 있는 탭을 다시 그림
    private void RefreshCurrentTab()
    {
        if (EquipmentTabToggle.isOn) ShowEquipmentTab();
        else if (ConsumableTabToggle.isOn) ShowConsumableTab();
        else if (EtcTabToggle.isOn) ShowEtcTab();
    }

    private void UpdateGoldUI()
    {
        if (goldText == null) return;
        var goldSystem = FindAnyObjectByType<GoldSystem>();
        goldText.text = goldSystem != null ? goldSystem.GetBalance().ToString() : "0";
    }


    #region 슬롯 클릭/선택 & 툴팁
    private void SubscribeSlotClicks(IEnumerable<InventorySlotUI> slots)
    {
        foreach (var slot in slots)
        {
            slot.onClick = OnSlotClicked;
        }
    }

    private void OnSlotClicked(InventorySlotUI slot, Vector2 screenPos)
    {
        // 빈 슬롯이면 선택 해제
        if (slot == null || slot.ItemData == null)
        {
            SelectSlot(null);
            return;
        }

        // 선택 갱신
        SelectSlot(slot);

        // 툴팁 표시
        if (toolTipUI != null)
        {
            toolTipUI.Set(slot.ItemData, screenPos);
        }
    }

    private void SelectSlot(InventorySlotUI slot)
    {
        _selectedSlot = slot;

        // 선택 해제 시 툴팁 닫기
        if (_selectedSlot == null && toolTipUI != null)
        {
            toolTipUI.Clear();
        }
    }
    #endregion

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
    public void SetWeaponSlot(EquipmentItemData equipmentItemData, bool flag)
    {
        switch (equipmentItemData.EquipType)
        {
            case EquipType.Weapon:
                {
                    weaponSlotImage.sprite = flag ? equipmentItemData.ItemSprite : null;
                    weaponSlotImage.enabled = flag;
                    break;
                }
            case EquipType.Body:
                {
                    bodySlotImage.sprite = flag ? equipmentItemData.ItemSprite : null;
                    bodySlotImage.enabled = flag;
                    break;
                }
            case EquipType.Accessory:
                {
                    accessoryImage.sprite = flag ? equipmentItemData.ItemSprite : null;
                    accessoryImage.enabled = flag;
                    break;
                }
        }

    }




}

