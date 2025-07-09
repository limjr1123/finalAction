using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필요
using System.Collections.Generic; // List 사용을 위해 필요

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("스폰될 몬스터 프리팹을 할당하세요.")]
    [SerializeField] private GameObject monsterPrefab; // 스폰할 몬스터 프리팹 (필수)

    [Tooltip("스폰할 몬스터의 총 마릿수.")]
    [SerializeField] private int numberOfMonstersToSpawn = 3; // 스폰할 몬스터 수

    [Tooltip("몬스터 스폰 간의 딜레이 (초).")]
    [SerializeField] private float spawnInterval = 0.5f; // 몬스터 스폰 간 딜레이

    [Tooltip("몬스터가 스폰될 최대 범위 (스포너 중심으로 반지름).")]
    [SerializeField] private float spawnRadius = 5f; // 몬스터가 스폰될 범위 반지름

    [Tooltip("플레이어가 스포너를 트리거할 대상입니다. 태그를 'Player'로 설정하세요.")]
    [SerializeField] private GameObject player; // 플레이어 오브젝트 (Inspector에서 할당 또는 "Player" 태그로 찾음)

    [Tooltip("스포너 트리거에 진입 후 몬스터 스폰 시작까지의 딜레이 (초).")]
    [SerializeField] private float triggerDelay = 1.0f; // 플레이어 진입 후 스폰 시작까지의 딜레이

    [Header("Debugging & State")]
    [SerializeField] private bool hasSpawned = false; // 이미 스폰이 완료되었는지 여부
    private List<GameObject> spawnedMonsters = new List<GameObject>(); // 스폰된 몬스터들을 추적하기 위한 리스트

    void Start()
    {
        // 플레이어 오브젝트가 할당되지 않았다면, "Player" 태그를 가진 오브젝트를 찾음
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("MonsterSpawner: Player GameObject not found. Please assign it in the Inspector or tag your player with 'Player'.");
            }
        }

        // 스폰할 몬스터 프리팹이 할당되지 않았다면 경고
        if (monsterPrefab == null)
        {
            Debug.LogError("MonsterSpawner: Monster Prefab is not assigned! This spawner cannot function.", this);
            this.enabled = false; // 스크립트 비활성화
        }

        // 스포너 오브젝트에 Collider가 없거나 Is Trigger가 활성화되지 않았다면 경고
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("MonsterSpawner: This spawner needs a Collider component with 'Is Trigger' enabled to detect player entry.", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 스포너 범위에 들어왔고, 아직 스폰되지 않았다면
        if (other.gameObject == player && !hasSpawned)
        {
            Debug.Log($"플레이어 '{player.name}'가 스포너 '{gameObject.name}' 범위에 진입했습니다. {triggerDelay}초 후 몬스터 스폰 시작.");
            // 일정 딜레이 후 몬스터 스폰 코루틴 시작
            Invoke("StartSpawningCoroutine", triggerDelay);
        }
    }

    void StartSpawningCoroutine()
    {
        if (!hasSpawned) // 중복 호출 방지
        {
            StartCoroutine(SpawnMonsters());
            hasSpawned = true; // 스폰 시작 플래그 설정
        }
    }

    IEnumerator SpawnMonsters()
    {
        Debug.Log($"몬스터 스폰 시작! {numberOfMonstersToSpawn} 마리 스폰 예정.");

        // (선택 사항) DungeonManager가 있다면 총 몬스터 수 업데이트
        // 이 스포너가 던전의 유일한 스포너라면 여기서 totalMonstersInDungeon을 설정

        ////if (DungeonManager.Instance != null)
        ////{
        ////    DungeonManager.Instance.SetTotalMonsters(numberOfMonstersToSpawn);
        ////}


        for (int i = 0; i < numberOfMonstersToSpawn; i++)
        {
            // 스폰 위치 계산 (스포너 주변 spawnRadius 내의 랜덤 위치)
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius; // 구형 범위 내 랜덤 지점
            randomOffset.y = 0; // Y축은 0으로 고정하여 바닥에 스폰 (필요에 따라 Y축 오프셋 추가 가능)
            Vector3 spawnPosition = transform.position + randomOffset;

            // 지형 높이에 맞추기 (선택 사항, Terrain이나 NavMesh 사용 시)
            // Raycast를 아래로 쏴서 지형에 닿는 위치를 찾거나, NavMesh.SamplePosition 사용
            // 예:
            /*
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, 100f, LayerMask.GetMask("Ground")))
            {
                spawnPosition.y = hit.point.y + 0.1f; // 지형 약간 위
            }
            */
            // 또는 NavMesh 사용 시:
            /*
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPosition, out navHit, spawnRadius, UnityEngine.AI.NavMesh.AllAreas))
            {
                spawnPosition = navHit.position;
            }
            */

            // 몬스터 인스턴스화 (스폰)
            GameObject spawnedMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
            spawnedMonsters.Add(spawnedMonster); // 스폰된 몬스터 리스트에 추가

            Debug.Log($"몬스터 '{spawnedMonster.name}' 스폰됨. (위치: {spawnPosition})");

            // 다음 몬스터 스폰까지 딜레이
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("모든 몬스터 스폰 완료!");
        // 스폰 완료 후 스포너 비활성화 (선택 사항, 필요에 따라)
        // gameObject.SetActive(false);
    }

    // 유니티 에디터에서 스폰 범위 시각화
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // 스포너의 위치에 와이어 스피어 그리기
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    // (옵션) 스폰된 몬스터들을 외부에서 추적할 필요가 있다면
    public List<GameObject> GetSpawnedMonsters()
    {
        return spawnedMonsters;
    }
}