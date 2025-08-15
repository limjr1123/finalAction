using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : BaseUI
{
    [Header("탭 토글")]
    [SerializeField] private Toggle equipmentToggle;
    [SerializeField] private Toggle consumptionToggle;

    [Header("장비 슬롯 부모 오브젝트")]
    [SerializeField] private GameObject equipmentPanel; // 장비 슬롯들이 들어있는 패널

    [Header("소비 슬롯 부모 오브젝트")]
    [SerializeField] private GameObject consumptionPanel; // 소비 슬롯들이 들어있는 패널

    [Header("버튼")]
    [SerializeField] private Button closeButton;

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI goldText;

    private ShopItemSlotUI[] equipmentSlots;
    private ShopItemSlotUI[] consumptionSlots;

    private void Awake()
    {
        equipmentToggle.onValueChanged.AddListener(OnEquipmentToggle);
        consumptionToggle.onValueChanged.AddListener(OnConsumptionToggle);

        equipmentSlots = equipmentPanel.GetComponentsInChildren<ShopItemSlotUI>(true);
        consumptionSlots = consumptionPanel.GetComponentsInChildren<ShopItemSlotUI>(true);
    }

    private void OnEnable()
    {
        // 상점 창이 열릴 때 골드 동기화
        UpdateGoldUI();
    }

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventoryUI);

        // 시작 시 장비탭이 켜져있으면 그 패널만 On
        if (equipmentToggle.isOn)
        {
            ShowEquipmentPanel();
        }
        else
        {
            ShowConsumptionPanel();
        }

        // 시작 시 골드 표시 1회 보정
        UpdateGoldUI();
    }

    private void CloseInventoryUI()
    {
        CloseUI();
    }

    private void OnEquipmentToggle(bool isOn)
    {
        if (isOn)
        {
            ShowEquipmentPanel();
            UpdateGoldUI(); // 탭 전환 시도 때도 갱신
        }
    }

    private void OnConsumptionToggle(bool isOn)
    {
        if (isOn)
        {
            ShowConsumptionPanel();
            UpdateGoldUI(); // 탭 전환 시도 때도 갱신
        }
    }

    private void ShowEquipmentPanel()
    {
        equipmentPanel.SetActive(true);
        consumptionPanel.SetActive(false);
        RefreshEquipmentSlots();
    }

    private void ShowConsumptionPanel()
    {
        equipmentPanel.SetActive(false);
        consumptionPanel.SetActive(true);
        RefreshConsumptionSlots();
    }

    private void RefreshEquipmentSlots()
    {
        var shop = ShopInstaller.ShopInstance;
        if (shop == null) return;
        var allItems = shop.GetAllItems();

        // 1. 모두 비활성화
        foreach (var slot in equipmentSlots) slot.SetActive(false);

        // 2. 장비 아이템만 슬롯에 세팅
        int idx = 0;
        foreach (var item in allItems)
        {
            if (item.ItemType != ItemType.Equipment) continue;
            if (idx >= equipmentSlots.Length) break;
            equipmentSlots[idx].Set(item, this);
            equipmentSlots[idx].SetActive(true);
            idx++;
        }
    }

    private void RefreshConsumptionSlots()
    {
        var shop = ShopInstaller.ShopInstance;
        if (shop == null) return;
        var allItems = shop.GetAllItems();

        // 1. 모두 비활성화
        foreach (var slot in consumptionSlots) slot.SetActive(false);

        // 2. 소비 아이템만 슬롯에 세팅
        int idx = 0;
        foreach (var item in allItems)
        {
            if (item.ItemType != ItemType.Consumable) continue;
            if (idx >= consumptionSlots.Length) break;
            consumptionSlots[idx].Set(item, this);
            consumptionSlots[idx].SetActive(true);
            idx++;
        }
    }

    // 슬롯에서 구매 버튼 누를 때
    public void TryPurchase(string itemID)
    {
        var shop = ShopInstaller.ShopInstance;
        if (shop == null) return;

        if (shop.Purchase(itemID))
        {
            Debug.Log("구매 성공!");
            // 현재 활성화된 패널만 갱신
            if (equipmentPanel.activeSelf)
                RefreshEquipmentSlots();
            else if (consumptionPanel.activeSelf)
                RefreshConsumptionSlots();

            // 골드 감소 반영
            UpdateGoldUI();
        }
        else
        {
            Debug.Log("구매 실패(돈 부족)");
            // 실패해도 혹시 외부에서 변동됐을 수 있으니 한 번 동기화
            UpdateGoldUI();
        }
    }

    public void UpdateGoldUI()
    {
        if (goldText == null) return;
        var goldSystem = FindAnyObjectByType<GoldSystem>();
        goldText.text = goldSystem != null ? goldSystem.GetBalance().ToString() : "0";
    }
}
