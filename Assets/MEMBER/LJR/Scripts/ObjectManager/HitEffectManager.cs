using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum HitEffectType
{
    Hit,        // 일반 히트 이펙트
    Slash,      // 슬래시 이펙트
    Block,      // 블록 이펙트
    AttackReady,// 공격 준비 이펙트
}

public class HitEffectManager : MonoBehaviour
{
    public static HitEffectManager Instance { get; private set; }

    [Header("Hit Effect Prefabs")]
    [SerializeField] private GameObject hitEffectPrefab;        // 히트 이펙트 프리팹
    [SerializeField] private GameObject slashEffectPrefab;      // 슬래시 이펙트 프리팹
    [SerializeField] private GameObject blockEffectPrefab;      // 가드 이펙트 프리팹

    [Header("Attack Ready Effect")]
    [SerializeField] private GameObject attackReadyEffectPrefab;  // 가불기 전조 이펙트 프리팹

    private Dictionary<HitEffectType, GameObject> effectPrefabs;
    private Dictionary<HitEffectType, AudioClip> audioClips;
    private Dictionary<HitEffectType, Queue<GameObject>> effectPools;

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 10; // 초기 풀 크기

    [Header("Scene Settings")]
    [SerializeField] private string[] allowedScenes = { "Dungeon", "LJR_Monster" }; // DontDestroyOnLoad를 적용할 씬들

    [Header("Audio Settings")]
    [SerializeField] AudioClip hit; // 오디오 클립
    [SerializeField] AudioClip slash; // 오디오 클립 배열
    [SerializeField] AudioClip block; // 오디오 클립 배열
    [SerializeField] AudioClip attackReady; // 오디오 클립 배열

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 현재 씬이 허용된 씬 목록에 있는지 확인
            string currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"Current Scene: {currentSceneName}");
            if (IsSceneAllowed(currentSceneName))
            {
                DontDestroyOnLoad(gameObject); // 허용된 씬에서만 파괴되지 않도록 설정
            }
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }

        effectPrefabs = new Dictionary<HitEffectType, GameObject>
        {
            { HitEffectType.Hit, hitEffectPrefab },
            { HitEffectType.Slash, slashEffectPrefab },
            { HitEffectType.Block, blockEffectPrefab },
            { HitEffectType.AttackReady, attackReadyEffectPrefab }
        };
        audioClips = new Dictionary<HitEffectType, AudioClip>
        {
            { HitEffectType.Hit, hit },
            { HitEffectType.Slash, slash },
            { HitEffectType.Block, block },
            { HitEffectType.AttackReady, attackReady }
        };

        effectPools = new Dictionary<HitEffectType, Queue<GameObject>>();
    }

    // 현재 씬이 허용된 씬 목록에 있는지 확인하는 메서드
    private bool IsSceneAllowed(string currentSceneName)
    {
        foreach (string allowedScene in allowedScenes)
        {
            if (currentSceneName == allowedScene)
            {
                return true;
            }
        }
        return false;
    }

    void Start()
    {
        InitializeEffectPool();
    }

    // 이펙트 풀 초기화
    private void InitializeEffectPool()
    {
        foreach (var type in effectPrefabs.Keys)
        {
            effectPools[type] = new Queue<GameObject>();
            if (type == HitEffectType.AttackReady)
            {
                GameObject effect = Instantiate(effectPrefabs[type]);
                effect.SetActive(false);
                effect.transform.SetParent(transform);
                effectPools[type].Enqueue(effect);
            }
            else
            {
                for (int i = 0; i < initialPoolSize; i++) // 초기 풀 크기 설정
                {
                    GameObject effect = Instantiate(effectPrefabs[type]);
                    effect.SetActive(false);
                    effect.transform.SetParent(transform);
                    effectPools[type].Enqueue(effect);
                }
            }
        }
    }

    // 이펙트 생성
    public void EffectCreate(Transform target, HitEffectType effectType = HitEffectType.Hit, Vector3? offset = null, Quaternion? rotation = null)
    {
        Debug.Log("이펙트 동작");
        GameObject effect = GetEffectFromPool(effectType);
        effect.transform.position = target.position + (offset ?? Vector3.zero);
        effect.transform.rotation = rotation ?? Quaternion.identity;
        effect.SetActive(true);

        //사운드 이펙트 추가
        SoundManager.Instance.PlaySkillSFX(audioClips[effectType]);

        if (effectType == HitEffectType.AttackReady)
        {
            effect.transform.SetParent(target); // 공격 준비 이펙트는 타겟에 부모로 설정
        }

        // 파티클 반환 처리
        var particle = effect.GetComponent<ParticleSystem>();
        if (particle != null)
        {
            StartCoroutine(ReturnEffectAfterDuration(effect, effectType, particle.main.duration));
        }
    }

    

    // 이펙트 풀에서 이펙트를 가져오는 메서드
    private GameObject GetEffectFromPool(HitEffectType type)
    {
        if (effectPools[type].Count > 0)
        {
            return effectPools[type].Dequeue();
        }
        else
        {
            return Instantiate(effectPrefabs[type]);
        }
    }

    // 이펙트를 일정 시간 후에 풀로 반환하는 코루틴
    private IEnumerator ReturnEffectAfterDuration(GameObject effect, HitEffectType type, float duration)
    {
        yield return new WaitForSeconds(duration);
        effect.SetActive(false);
        effectPools[type].Enqueue(effect);

        if(type == HitEffectType.AttackReady)
            effect.transform.SetParent(transform);
    }
}
