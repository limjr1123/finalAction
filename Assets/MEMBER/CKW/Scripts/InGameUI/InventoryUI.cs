using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : BaseUI
{

    [SerializeField] Button closeButton;

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
