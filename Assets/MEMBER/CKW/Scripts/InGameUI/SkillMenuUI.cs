using UnityEngine;
using UnityEngine.UI;

public class SkillMenuUI : BaseUI
{
    [SerializeField] Button closeButton;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSkillMenuUI);
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void CloseSkillMenuUI()
    {
        CloseUI();
    }

}
