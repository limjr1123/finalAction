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

    }


    private void CloseInventoryUI()
    {
        CloseUI();
    }


    private void SellItem()
    {

    }


    private void OnEquipmentTabToggle(bool ison)
    {
        EquipmentSlotArea.SetActive(true);
    }


    private void OnConsumableTabToggle(bool ison)
    {
        ConsumableSlotArea.SetActive(true);
    }


    private void OnEtcTabToggle(bool ison)
    {
        EtcSlotArea.SetActive(true);
    }

}
