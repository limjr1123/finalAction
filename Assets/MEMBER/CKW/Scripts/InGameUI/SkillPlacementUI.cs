using UnityEngine;
using UnityEngine.UI;

public class SkillPlacementUI : BaseUI
{
    [SerializeField] Button closeButton;



    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSkillPlacementUI);
    }


    void CloseSkillPlacementUI()
    {
        CloseUI();
    }
}
