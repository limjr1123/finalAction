using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


#region  BGM 모음
public enum BGMList
{
    CKW_TitleScene,
    CKW_CharacterSelectScene,       // 캐릭터 선택 씬
    Field,                          // 필드(첫 맵) 씬
    Dungeon

}
#endregion

#region  UI 효과음 모음
public enum UISFXList
{
    Server,
    Button,
    Select

}
#endregion

[System.Serializable]
public class VolumeSettings
{
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float skillSFXVolume = 1f;
    public float uiSFXVolume = 1f;

    public bool masterMute = false;
    public bool bgmMute = false;
    public bool skillSFXMute = false;
    public bool uiSFXMute = false;
}
public class SoundManager : Singleton<SoundManager>
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource[] skillSFXSource;
    [SerializeField] AudioSource uiSFXSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] bgmClips;
    [SerializeField] AudioClip[] uiSFXClips;

    private VolumeSettings volumeSettings = new VolumeSettings();   // 볼륨 설정
    private string saveFilePath;                                    // 저장 파일 경로
    private string currentScene;
    private AudioClip currentBGM;



    protected override void Awake()
    {
        base.Awake();
        saveFilePath = Path.Combine(Application.persistentDataPath, "VolumeSettings.json");     // 경로에 json파일 연결
        LoadVolumeSettings();

        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.playOnAwake = true;
            // 초기 볼륨 설정 시 음소거 상태 고려
            float finalVolume = volumeSettings.masterMute || volumeSettings.bgmMute ? 0f : volumeSettings.bgmVolume * volumeSettings.masterVolume;
            bgmSource.volume = finalVolume;
        }

        if (skillSFXSource != null)
        {
            for (int i = 0; i < skillSFXSource.Length; i++)
            {
                skillSFXSource[i].loop = false;
                skillSFXSource[i].playOnAwake = false;
                // 초기 볼륨 설정 시 음소거 상태 고려
                float finalVolume = volumeSettings.masterMute || volumeSettings.skillSFXMute ? 0f :
                                   volumeSettings.skillSFXVolume * volumeSettings.masterVolume;
                skillSFXSource[i].volume = finalVolume;
            }
        }

        if (uiSFXSource != null)
        {
            uiSFXSource.loop = false;
            uiSFXSource.playOnAwake = false;
            // 초기 볼륨 설정 시 음소거 상태 고려
            float finalVolume = volumeSettings.masterMute || volumeSettings.uiSFXMute ? 0f :
                               volumeSettings.uiSFXVolume * volumeSettings.masterVolume;
            uiSFXSource.volume = finalVolume;
        }
        UpdateAllVolumes();
    }

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        PlaySceneBGM(currentScene);
    }


    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != currentScene)
        {
            currentScene = sceneName;
            PlaySceneBGM(currentScene);
        }
    }


    void LoadVolumeSettings()   // 볼륨 설정을 JSON 파일에서 로드
    {
        try
        {
            if (File.Exists(saveFilePath))  // 경로에 파일이 존재하면 true반환
            {
                string json = File.ReadAllText(saveFilePath);                   // 해당 파일의 내용을 문자열로 읽어옴 (JSON 텍스트)
                volumeSettings = JsonUtility.FromJson<VolumeSettings>(json);    // JSON 텍스트를 VolumeSettings 타입 객체로 변환 (역직렬화)
                Debug.Log("저장된 값 불러오기 성공이요");
            }
            else
            {
                Debug.Log("저장된 볼륨이 없다yo");
                SaveVolumeSettings();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"볼륨 설정 로드 실패: {e.Message}");
            volumeSettings = new VolumeSettings(); // 오류 시 기본값 사용
        }
    }


    void SaveVolumeSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(volumeSettings);
            File.WriteAllText(saveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.Log($"저장 실패 {e.Message}");
        }

    }


    public void ForceUpdateAllVolumes()
    {
        UpdateAllVolumes();
        Debug.Log("볼륨 강제 업데이트 완료");
    }


    void UpdateAllVolumes()
    {
        if (bgmSource != null)
        {
            float finalVolume = volumeSettings.masterMute || volumeSettings.bgmMute ? 0f :
                               volumeSettings.bgmVolume * volumeSettings.masterVolume;
            bgmSource.volume = finalVolume;
        }

        if (skillSFXSource != null)
        {
            float finalVolume = volumeSettings.masterMute || volumeSettings.skillSFXMute ? 0f :
                               volumeSettings.skillSFXVolume * volumeSettings.masterVolume;
            for (int i = 0; i < skillSFXSource.Length; i++)
            {
                skillSFXSource[i].volume = finalVolume;
            }
        }

        if (uiSFXSource != null)
        {
            float finalVolume = volumeSettings.masterMute || volumeSettings.uiSFXMute ? 0f :
                               volumeSettings.uiSFXVolume * volumeSettings.masterVolume;
            uiSFXSource.volume = finalVolume;
        }
    }


    #region BGM 플레이
    void PlayBGM(AudioClip _clip)
    {
        if (currentBGM == _clip && bgmSource.isPlaying)
            return;
        if (bgmSource != null && bgmSource != null)
        {
            bgmSource.clip = _clip;
            bgmSource.Play();
            currentBGM = _clip;
        }
    }


    public void PlaySceneBGM(string _sceneName)
    {
        if (Enum.TryParse<BGMList>(_sceneName, out var bgmType))
        {
            int index = (int)bgmType;
            if (index >= 0 && index <= bgmClips.Length)
                PlayBGM(bgmClips[index]);
            else
                Debug.Log("설정된 BGM 인덱스가 없습니다.");
        }
    }


    public void StopBGM()
    {
        bgmSource.Stop();
    }


    public void PauseBGM()
    {
        bgmSource.Pause();
    }


    public void UnPauseBGM()
    {
        bgmSource.UnPause();
    }

    #endregion

    #region  스킬 SFX 플레이
    public void PlaySkillSFX(AudioClip _clip)
    {
        if (skillSFXSource != null && _clip != null)
        {
            for (int i = 0; i < skillSFXSource.Length; i++)
            {
                if (!skillSFXSource[i].isPlaying)
                {
                    skillSFXSource[i].PlayOneShot(_clip);
                    return;
                }
            }
        }
    }
    #endregion

    #region  UI SFX 플레이
    public void PlayUISFX(UISFXList _uiSFX)
    {
        Debug.Log($"PlayUISFX 호출됨: {_uiSFX}");

        if (uiSFXSource != null && uiSFXClips != null)
        {
            int index = (int)_uiSFX;
            Debug.Log($"인덱스: {index}, 클립 배열 길이: {uiSFXClips.Length}");

            if (index < uiSFXClips.Length && index >= 0)
            {
                if (uiSFXClips[index] != null)
                {
                    Debug.Log($"오디오 클립 재생: {uiSFXClips[index].name}");
                    uiSFXSource.PlayOneShot(uiSFXClips[index]);
                }
                else
                {
                    Debug.Log("오디오 클립이 null입니다");
                }
            }
            else
            {
                Debug.Log("인덱스가 범위를 벗어남");
            }
        }
        else
        {
            Debug.Log($"uiSFXSource: {uiSFXSource}, uiSFXClips: {uiSFXClips}");
        }
    }
    #endregion

    #region  볼륨 조절 함수
    public void SetMasterVolume(float value)
    {
        volumeSettings.masterVolume = Mathf.Clamp01(value);
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public void SetBGMVolume(float value)
    {
        volumeSettings.bgmVolume = Mathf.Clamp01(value);
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public void SetSkillSFXVolume(float value)
    {
        volumeSettings.skillSFXVolume = Mathf.Clamp01(value);
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public void SetUISFXVolume(float value)
    {
        volumeSettings.uiSFXVolume = Mathf.Clamp01(value);
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public float GetMasterVolume() => volumeSettings.masterVolume;
    public float GetBGMVolume() => volumeSettings.bgmVolume;
    public float GetSkillSFXVolume() => volumeSettings.skillSFXVolume;
    public float GetUISFXVolume() => volumeSettings.uiSFXVolume;

    #endregion

    #region 음소거 설정 함수
    public void SetMasterMute(bool isMuted)
    {
        volumeSettings.masterMute = isMuted;
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public void SetBGMMute(bool isMuted)
    {
        volumeSettings.bgmMute = isMuted;
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public void SetSkillSFXMute(bool isMuted)
    {
        volumeSettings.skillSFXMute = isMuted;
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public void SetUISFXMute(bool isMuted)
    {
        volumeSettings.uiSFXMute = isMuted;
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    // 음소거 상태 가져오기 함수들
    public bool GetMasterMute() => volumeSettings.masterMute;
    public bool GetBGMMute() => volumeSettings.bgmMute;
    public bool GetSkillSFXMute() => volumeSettings.skillSFXMute;
    public bool GetUISFXMute() => volumeSettings.uiSFXMute;
    #endregion
}
