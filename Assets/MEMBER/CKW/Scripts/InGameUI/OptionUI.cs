using UnityEngine;
using UnityEngine.UI;

public class OptionUI : BaseUI
{
    [SerializeField] Button closeButton;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseOptionUI);
    }


    private void CloseOptionUI()
    {
        CloseUI();
    }
}