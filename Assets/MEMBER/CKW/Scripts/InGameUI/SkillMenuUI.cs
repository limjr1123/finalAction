using UnityEngine;
using UnityEngine.UI;

public class SkillMenuUI : BaseUI
{
    [SerializeField] Button closeButton;
    [SerializeField] Button skiilPlacementButton;

    [SerializeField] GameObject skillPlacement;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSkillMenuUI);

        if (skiilPlacementButton != null)
            skiilPlacementButton.onClick.AddListener(OnSkillPlacementUI);
    }


    private void CloseSkillMenuUI()
    {
        CloseUI();
    }


    private void OnSkillPlacementUI()
    {
        BaseUI skillPlacementUI = skillPlacement.GetComponent<BaseUI>();

        if (skillPlacement != null)
            skillPlacementUI.OpenUI();
    }

}
