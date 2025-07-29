using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    [SerializeField] private GameObject WarriorPrefab;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject thiefPrefab;

    private Dictionary<string, GameObject> prefabMap;


    protected override void Awake()
    {
        base.Awake();

        prefabMap = new Dictionary<string, GameObject>();

        prefabMap.Add("전사", WarriorPrefab);
        prefabMap.Add("궁수", archerPrefab);
        prefabMap.Add("마법사", magePrefab);
        prefabMap.Add("도적", thiefPrefab);
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
        // 캐릭터 인스턴스 생성 후에 데이터 로드하기

        // 1. 플레이어 인스턴스 생성하기
        Debug.Log("이름 : " + playerSaveData.characterJob);
        GameObject player = prefabMap[playerSaveData.characterJob];
        Instantiate(player);

        // 2. 데이터 로드하기
        var playerStats = player.GetComponent<PlayerStats>();
        playerStats.LoadData(playerSaveData);
    }

    private PlayerSaveData GetChracterSaveData()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player null");
            return null;
        }

        PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats null");
            return null;
        }

        // PlayerStats를 이용해 PlayerSaveData 스냅샷을 만듭니다.
        PlayerSaveData newPlayerSaveData = new PlayerSaveData(playerStats);

        return newPlayerSaveData;
    }
}

