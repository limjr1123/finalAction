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

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventoryUI);
    }


    private void CloseInventoryUI()
    {
        CloseUI();
    }


}
