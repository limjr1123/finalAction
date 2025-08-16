using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

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

    [Header("Specific Mute Toggles")]
    [SerializeField] Toggle masterMuteOffUIToggle;
    [SerializeField] Toggle masterMuteOnUIToggle;
    [SerializeField] Toggle bgmMuteOffUIToggle;
    [SerializeField] Toggle bgmMuteOnUIToggle;
    [SerializeField] Toggle skillSFXMuteOffUIToggle;
    [SerializeField] Toggle skillSFXMuteOnUIToggle;
    [SerializeField] Toggle uiSFXMuteOffUIToggle;
    [SerializeField] Toggle uiSFXMuteOnUIToggle;

    // 사용할 색상 정의 (Inspector에서 설정 가능하도록 SerializeField 추가)
    [Header("Toggle Colors")]
    [SerializeField] Color selectedColor = new Color(100f / 255f, 100f / 255f, 200f / 255f, 1f); // 회색 (Is On일 때)
    [SerializeField] Color normalColor = new Color(1f, 1f, 1f, 1f); // 흰색 (Is On 아닐 때)

    void Start()
    {
        SetupButtons();
        SetupSliders();
        SetupToggles();

        StartCoroutine(InitializeUIWithDelay());
    }

    IEnumerator InitializeUIWithDelay()
    {
        yield return new WaitForEndOfFrame();

        InitializeSliderValues();
        InitializeToggleValues();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ForceUpdateAllVolumes();
        }
    }

    void SetupButtons()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseOptionUI);
        if (characterSelectButton != null)
            characterSelectButton.onClick.AddListener(MoveCharacterSelect);
        if (exitGameButton != null)
            exitGameButton.onClick.AddListener(ExitGame);
    }

    void SetupSliders()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnSetMasterVolume);
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnSetBGMVolume);
        if (skillSFXSlider != null)
            skillSFXSlider.onValueChanged.AddListener(OnSetSkillSFXVolume);
        if (uiSFXSlider != null)
            uiSFXSlider.onValueChanged.AddListener(OnSetUISFXVolume);
    }

    void SetupToggles()
    {
        if (masterMuteOffUIToggle != null)
            masterMuteOffUIToggle.onValueChanged.AddListener((isOn) => OnToggleMasterMute(isOn));
        if (masterMuteOnUIToggle != null)
            masterMuteOnUIToggle.onValueChanged.AddListener((isOn) => OnToggleMasterMute(!isOn)); // !isOn으로 변경

        if (bgmMuteOffUIToggle != null)
            bgmMuteOffUIToggle.onValueChanged.AddListener((isOn) => OnToggleBGMMute(isOn));
        if (bgmMuteOnUIToggle != null)
            bgmMuteOnUIToggle.onValueChanged.AddListener((isOn) => OnToggleBGMMute(!isOn));

        if (skillSFXMuteOffUIToggle != null)
            skillSFXMuteOffUIToggle.onValueChanged.AddListener((isOn) => OnToggleSkillSFXMute(isOn));
        if (skillSFXMuteOnUIToggle != null)
            skillSFXMuteOnUIToggle.onValueChanged.AddListener((isOn) => OnToggleSkillSFXMute(!isOn));

        if (uiSFXMuteOffUIToggle != null)
            uiSFXMuteOffUIToggle.onValueChanged.AddListener((isOn) => OnToggleUISFXMute(isOn));
        if (uiSFXMuteOnUIToggle != null)
            uiSFXMuteOnUIToggle.onValueChanged.AddListener((isOn) => OnToggleUISFXMute(!isOn));
    }

    void InitializeSliderValues()
    {
        if (SoundManager.Instance == null) return;
        RemoveSliderListeners();
        if (masterVolumeSlider != null) masterVolumeSlider.value = SoundManager.Instance.GetMasterVolume();
        if (bgmSlider != null) bgmSlider.value = SoundManager.Instance.GetBGMVolume();
        if (skillSFXSlider != null) skillSFXSlider.value = SoundManager.Instance.GetSkillSFXVolume();
        if (uiSFXSlider != null) uiSFXSlider.value = SoundManager.Instance.GetUISFXVolume();
        SetupSliders();
    }

    void InitializeToggleValues()
    {
        if (SoundManager.Instance == null) return;

        RemoveToggleListeners();

        // 마스터 볼륨 토글 초기화
        if (masterMuteOffUIToggle != null && masterMuteOnUIToggle != null)
        {
            bool isMuted = SoundManager.Instance.GetMasterMute();
            if (isMuted) // 저장된 상태가 "음소거 됨" (MasterMuteOffUIToggle이 true가 되어야 할 때)
            {
                masterMuteOffUIToggle.SetIsOnWithoutNotify(true); // '끄기' 토글을 ON으로 설정
            }
            else // 저장된 상태가 "음소거 아님" (MasterMuteOnUIToggle이 true가 되어야 할 때)
            {
                masterMuteOnUIToggle.SetIsOnWithoutNotify(true); // '켜기' 토글을 ON으로 설정
            }
            UpdateToggleVisuals(masterMuteOffUIToggle, masterMuteOnUIToggle);
            Debug.Log($"Master Mute Toggle 초기화: {isMuted}");
        }

        // BGM 볼륨 토글 초기화
        if (bgmMuteOffUIToggle != null && bgmMuteOnUIToggle != null)
        {
            bool isMuted = SoundManager.Instance.GetBGMMute();
            if (isMuted)
            {
                bgmMuteOffUIToggle.SetIsOnWithoutNotify(true);
            }
            else
            {
                bgmMuteOnUIToggle.SetIsOnWithoutNotify(true);
            }
            UpdateToggleVisuals(bgmMuteOffUIToggle, bgmMuteOnUIToggle);
            Debug.Log($"BGM Mute Toggle 초기화: {isMuted}");
        }

        // 스킬 SFX 볼륨 토글 초기화
        if (skillSFXMuteOffUIToggle != null && skillSFXMuteOnUIToggle != null)
        {
            bool isMuted = SoundManager.Instance.GetSkillSFXMute();
            if (isMuted)
            {
                skillSFXMuteOffUIToggle.SetIsOnWithoutNotify(true);
            }
            else
            {
                skillSFXMuteOnUIToggle.SetIsOnWithoutNotify(true);
            }
            UpdateToggleVisuals(skillSFXMuteOffUIToggle, skillSFXMuteOnUIToggle);
            Debug.Log($"Skill SFX Mute Toggle 초기화: {isMuted}");
        }

        // UI SFX 볼륨 토글 초기화
        if (uiSFXMuteOffUIToggle != null && uiSFXMuteOnUIToggle != null)
        {
            bool isMuted = SoundManager.Instance.GetUISFXMute();
            if (isMuted)
            {
                uiSFXMuteOffUIToggle.SetIsOnWithoutNotify(true);
            }
            else
            {
                uiSFXMuteOnUIToggle.SetIsOnWithoutNotify(true);
            }
            UpdateToggleVisuals(uiSFXMuteOffUIToggle, uiSFXMuteOnUIToggle);
            Debug.Log($"UI SFX Mute Toggle 초기화: {isMuted}");
        }

        SetupToggles();
    }

    // 토글의 시각적 상태를 업데이트하는 새로운 함수
    void UpdateToggleVisuals(Toggle offToggle, Toggle onToggle)
    {
        if (offToggle == null || onToggle == null) return;

        // '끄기' 토글이 활성화되어 있다면 (즉, 음소거 상태)
        if (offToggle.isOn)
        {
            if (offToggle.targetGraphic != null)
                offToggle.targetGraphic.color = selectedColor; // 선택된 토글은 회색
            if (onToggle.targetGraphic != null)
                onToggle.targetGraphic.color = normalColor; // 선택 안 된 토글은 흰색
        }
        // '켜기' 토글이 활성화되어 있다면 (즉, 음소거 해제 상태)
        else if (onToggle.isOn)
        {
            if (offToggle.targetGraphic != null)
                offToggle.targetGraphic.color = normalColor; // 선택 안 된 토글은 흰색
            if (onToggle.targetGraphic != null)
                onToggle.targetGraphic.color = selectedColor; // 선택된 토글은 회색
        }
    }


    void RemoveSliderListeners()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.RemoveAllListeners();
        if (bgmSlider != null) bgmSlider.onValueChanged.RemoveAllListeners();
        if (skillSFXSlider != null) skillSFXSlider.onValueChanged.RemoveAllListeners();
        if (uiSFXSlider != null) uiSFXSlider.onValueChanged.RemoveAllListeners();
    }

    void RemoveToggleListeners()
    {
        if (masterMuteOffUIToggle != null) masterMuteOffUIToggle.onValueChanged.RemoveAllListeners();
        if (masterMuteOnUIToggle != null) masterMuteOnUIToggle.onValueChanged.RemoveAllListeners();
        if (bgmMuteOffUIToggle != null) bgmMuteOffUIToggle.onValueChanged.RemoveAllListeners();
        if (bgmMuteOnUIToggle != null) bgmMuteOnUIToggle.onValueChanged.RemoveAllListeners();
        if (skillSFXMuteOffUIToggle != null) skillSFXMuteOffUIToggle.onValueChanged.RemoveAllListeners();
        if (skillSFXMuteOnUIToggle != null) skillSFXMuteOnUIToggle.onValueChanged.RemoveAllListeners();
        if (uiSFXMuteOffUIToggle != null) uiSFXMuteOffUIToggle.onValueChanged.RemoveAllListeners();
        if (uiSFXMuteOnUIToggle != null) uiSFXMuteOnUIToggle.onValueChanged.RemoveAllListeners();
    }

    #region 볼륨 조절 함수
    public void OnSetMasterVolume(float value)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetMasterVolume(value);
    }
    public void OnSetBGMVolume(float value)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetBGMVolume(value);
    }
    public void OnSetSkillSFXVolume(float value)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetSkillSFXVolume(value);
    }
    public void OnSetUISFXVolume(float value)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetUISFXVolume(value);
    }
    #endregion

    #region 음소거 토글 함수
    public void OnToggleMasterMute(bool isOn)
    {
        // masterMuteOffUIToggle이 현재 On인지 확인하여 isMuted 상태를 결정
        bool isMuted = masterMuteOffUIToggle.isOn;
        Debug.Log($"Master Mute 토글: {isMuted}");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMasterMute(isMuted);
        }
        // 클릭 후 시각적 업데이트 강제
        UpdateToggleVisuals(masterMuteOffUIToggle, masterMuteOnUIToggle);
    }

    public void OnToggleBGMMute(bool isOn)
    {
        bool isMuted = bgmMuteOffUIToggle.isOn;
        Debug.Log($"BGM Mute 토글: {isMuted}");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMMute(isMuted);
        }
        UpdateToggleVisuals(bgmMuteOffUIToggle, bgmMuteOnUIToggle);
    }

    public void OnToggleSkillSFXMute(bool isOn)
    {
        bool isMuted = skillSFXMuteOffUIToggle.isOn;
        Debug.Log($"Skill SFX Mute 토글: {isMuted}");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSkillSFXMute(isMuted);
        }
        UpdateToggleVisuals(skillSFXMuteOffUIToggle, skillSFXMuteOnUIToggle);
    }

    public void OnToggleUISFXMute(bool isOn)
    {
        bool isMuted = uiSFXMuteOffUIToggle.isOn;
        Debug.Log($"UI SFX Mute 토글: {isMuted}");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetUISFXMute(isMuted);
        }
        UpdateToggleVisuals(uiSFXMuteOffUIToggle, uiSFXMuteOnUIToggle);
    }
    #endregion

    private void CloseOptionUI()
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Select);
        CloseUI();
    }

    private void MoveCharacterSelect()
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
        SceneLoader.LoadSceneAsync("CKW_CharacterSelectScene");
    }

    private void ExitGame()
    {
        SoundManager.Instance.PlayUISFX(UISFXList.Button);
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}