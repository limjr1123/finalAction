using UnityEngine;
using UnityEngine.UI;



public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] Button inventoryButton;
    [SerializeField] Button SkillWindowButton;
    [SerializeField] Button CharacterInfoButton;
    [SerializeField] Button ShopButton;
    [SerializeField] Button OptionButton;

    [Header("UI References")]
    [SerializeField] GameObject inventoryUIObject;
    [SerializeField] GameObject skillWindowUIObject;
    [SerializeField] GameObject characterInfoUIObject;
    [SerializeField] GameObject shopUIObject;
    [SerializeField] GameObject optionUIObject;

    void Start()
    {

        if (inventoryButton != null)
            inventoryButton.onClick.AddListener(OpenInventoryUI);
        if (SkillWindowButton != null)
            SkillWindowButton.onClick.AddListener(OpenSkillWindowUI);
        if (CharacterInfoButton != null)
            CharacterInfoButton.onClick.AddListener(OpenCharacterInfoUI);
        if (ShopButton != null)
            ShopButton.onClick.AddListener(OpenShopUI);
        if (OptionButton != null)
            OptionButton.onClick.AddListener(OpenOptionUI);


    }


    private void OpenInventoryUI()
    {
        if (UIManager.Instance != null && inventoryUIObject != null)
        {
            BaseUI inventoryBaseUI = inventoryUIObject.GetComponent<BaseUI>();
            if (inventoryBaseUI != null)
            {
                inventoryBaseUI.OpenUI();
            }
        }
    }

    private void OpenSkillWindowUI()
    {
        if (UIManager.Instance != null && skillWindowUIObject != null)
        {
            BaseUI skillWindowBaseUI = skillWindowUIObject.GetComponent<BaseUI>();
            if (skillWindowBaseUI != null)
            {
                skillWindowBaseUI.OpenUI();
            }
        }
    }

    private void OpenCharacterInfoUI()
    {
        if (UIManager.Instance != null && characterInfoUIObject != null)
        {
            BaseUI characterInfoBaseUI = characterInfoUIObject.GetComponent<BaseUI>();
            if (characterInfoBaseUI != null)
            {
                characterInfoBaseUI.OpenUI();
            }
        }
    }

    private void OpenShopUI()
    {
        if (UIManager.Instance != null && shopUIObject != null)
        {
            BaseUI shopBaseUI = shopUIObject.GetComponent<BaseUI>();
            if (shopBaseUI != null)
            {
                shopBaseUI.OpenUI();
            }
        }
    }

    private void OpenOptionUI()
    {
        if (UIManager.Instance != null && optionUIObject != null)
        {
            BaseUI optionBaseUI = optionUIObject.GetComponent<BaseUI>();
            if (optionBaseUI != null)
            {
                optionBaseUI.OpenUI();
            }
        }
    }


}