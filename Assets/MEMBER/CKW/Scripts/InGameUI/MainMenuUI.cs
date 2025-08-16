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

    // 캐릭터 정보 표시용 UI (필요시 추가)
    [Header("Character Info Display")]
    [SerializeField] Text characterNameText;
    [SerializeField] Image characterPortrait;

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

    // 캐릭터 데이터로 MainMenu 초기화 (UIManager에서 호출)
    public void InitializeWithCharacterData(int characterIndex)
    {
        Debug.Log($"MainMenu 캐릭터 데이터 초기화 - 인덱스: {characterIndex}");

        // GameDataSaveLoadManager에서 캐릭터 데이터 가져오기
        var characterData = GameDataSaveLoadManager.Instance.GetCharacterData(characterIndex);

        if (characterData != null)
        {
            // UI 업데이트 예시 (실제 PlayerSaveData 구조에 맞게 수정 필요)
            if (characterNameText != null)
                characterNameText.text = characterData.playerSaveData.characterName;
            // if (characterPortrait != null)
            //     characterPortrait.sprite = characterData.playerSaveData.portraitSprite;

            Debug.Log($"MainMenu 업데이트 완료 - 캐릭터: {characterData.playerSaveData.characterName}");
        }
        else
        {
            Debug.LogWarning($"캐릭터 데이터를 찾을 수 없습니다. 인덱스: {characterIndex}");
        }
    }

    private void OpenInventoryUI()
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
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
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
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
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
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
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
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
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
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