using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : BaseUI
{
    [SerializeField] Button closeButton;


    void Start()
    {
        closeButton.onClick.AddListener(CloseInfoUI);
    }


    private void CloseInfoUI()
    {
        CloseUI();
    }

}
