using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class OptionUI : BaseUI
{
    [Header("Buttons")]
    [SerializeField] Button closeButton;
    [SerializeField] Button characterSelectButton;
    [SerializeField] Button exitGameButton;


    [Header("Volume Sliders")]
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider skillSFXSlider;
    [SerializeField] Slider uiSFXSlider;



    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseOptionUI);
        if (characterSelectButton != null)
            characterSelectButton.onClick.AddListener(MoveCharacterSelect);
        if (exitGameButton != null)
            exitGameButton.onClick.AddListener(ExitGame);

        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnSetMasterVolume);
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnSetBGMVolume);
        if (skillSFXSlider != null)
            skillSFXSlider.onValueChanged.AddListener(OnSetSkillSFXVolume);
        if (uiSFXSlider != null)
            uiSFXSlider.onValueChanged.AddListener(OnSetUISFXVolume);

    }


    #region  볼륨 조절 함수
    public void OnSetMasterVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMasterVolume(value);
    }

    public void OnSetBGMVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetBGMVolume(value);
    }

    public void OnSetSkillSFXVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSkillSFXVolume(value);
    }


    public void OnSetUISFXVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetUISFXVolume(value);
    }
    #endregion


    private void CloseOptionUI()
    {
        CloseUI();
    }


    private void MoveCharacterSelect()
    {
        SceneManager.LoadScene(1);
    }


    private void ExitGame()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}