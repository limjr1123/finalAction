using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    [SerializeField] private GameObject WarriorPrefab;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject thiefPrefab;

    private Dictionary<string, GameObject> prefabMap;

    private GameObject _currentPlayerInstance;

    protected override void Awake()
    {
        base.Awake();

        prefabMap = new Dictionary<string, GameObject>
        {
            { "전사", WarriorPrefab },
            { "궁수", archerPrefab },
            { "마법사", magePrefab },
            { "도적", thiefPrefab }
        };
    }

    public PlayerSaveData SaveChracterData()
    {
        PlayerSaveData currentCharacterData = GetChracterSaveData();
        if (currentCharacterData == null)
        {
            Debug.LogError("데이터 스냅샷 생성에 실패하여 저장을 중단합니다.");
            return null;
        }

        return currentCharacterData;
    }

    public void LoadCharacterSaveData(PlayerSaveData playerSaveData)
    {
        // 0) 기존 캐릭터 있으면 정리
        if (_currentPlayerInstance != null)
            Destroy(_currentPlayerInstance);

        // 1) 프리팹 꺼내기
        if (!prefabMap.TryGetValue(playerSaveData.characterJob, out var prefab) || prefab == null)
        {
            Debug.LogError($"직업 {playerSaveData.characterJob} 프리팹을 찾을 수 없습니다.");
            return;
        }

        // 2) 인스턴스 생성하고 그 인스턴스에서 컴포넌트 뽑기
        _currentPlayerInstance = Instantiate(prefab);
        var playerStats = _currentPlayerInstance.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("생성된 플레이어에 PlayerStats가 없습니다.");
            return;
        }

        // 3) 데이터 로드
        playerStats.ApplyBaseStats();
        playerStats.LoadData(playerSaveData);

        // 4) 태그 부여
        _currentPlayerInstance.tag = "Player";
    }

    private PlayerSaveData GetChracterSaveData()
    {
        GameObject playerObject = _currentPlayerInstance;
        if (playerObject == null)
        {
            Debug.LogError("현재 플레이어 인스턴스가 없습니다.");
            return null;
        }

        var playerStats = playerObject.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats null");
            return null;
        }

        return new PlayerSaveData(playerStats);
    }
}