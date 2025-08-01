using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] Button equipButton;
    [SerializeField] Button liftButton;
    [SerializeField] GameObject toolTip;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemType;
    public TextMeshProUGUI itemText;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseUI);
        if (equipButton != null)
            equipButton.onClick.AddListener(ItemEquip);
        if (liftButton != null)
            liftButton.onClick.AddListener(ItemLift);
    }


    void OnCloseUI()
    {
        toolTip.SetActive(false);
    }


    void ItemEquip()
    {

    }


    void ItemLift()
    {

    }

}
