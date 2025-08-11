using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitEffectType
{
    Hit,        // 일반 히트 이펙트
    Slash,      // 슬래시 이펙트
    Block,      // 블록 이펙트
    AttackReady // 공격 준비 이펙트
}

public class HitEffectManager : MonoBehaviour
{
    public static HitEffectManager Instance { get; private set; }

    [Header("Hit Effect Prefabs")]
    [SerializeField] private GameObject hitEffectPrefab;    // 히트 이펙트 프리팹
    [SerializeField] private GameObject slashEffectPrefab;  // 슬래시 이펙트 프리팹
    [SerializeField] private GameObject blockEffectPrefab;  // 슬래시 이펙트 프리팹

    [Header("Attack Ready Effect")]
    [SerializeField] private GameObject attackReadyEffectPrefab;  // 슬래시 이펙트 프리팹

    private Dictionary<HitEffectType, GameObject> effectPrefabs;
    private Dictionary<HitEffectType, Queue<GameObject>> effectPools;

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 10; // 초기 풀 크기

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
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

        effectPools = new Dictionary<HitEffectType, Queue<GameObject>>();
    }

    void Start()
    {
        InitializeEffectPool();
    }

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

    public void EffectCreate(Transform target, HitEffectType effectType = HitEffectType.Hit, Vector3? offset = null, Quaternion? rotation = null)
    {
        GameObject effect = GetEffectFromPool(effectType);
        effect.transform.position = target.position + (offset ?? Vector3.zero);
        effect.transform.rotation = rotation ?? Quaternion.identity;
        effect.SetActive(true);

        // 파티클 반환 처리
        var particle = effect.GetComponent<ParticleSystem>();
        if (particle != null)
        {
            StartCoroutine(ReturnEffectAfterDuration(effect, effectType, particle.main.duration));
        }
    }

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

    private IEnumerator ReturnEffectAfterDuration(GameObject effect, HitEffectType type, float duration)
    {
        yield return new WaitForSeconds(duration);
        effect.SetActive(false);
        effectPools[type].Enqueue(effect);
    }
}
