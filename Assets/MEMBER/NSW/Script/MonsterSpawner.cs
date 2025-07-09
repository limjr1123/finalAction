using UnityEngine;
using System.Collections; // �ڷ�ƾ ����� ���� �ʿ�
using System.Collections.Generic; // List ����� ���� �ʿ�

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("������ ���� �������� �Ҵ��ϼ���.")]
    [SerializeField] private GameObject monsterPrefab; // ������ ���� ������ (�ʼ�)

    [Tooltip("������ ������ �� ������.")]
    [SerializeField] private int numberOfMonstersToSpawn = 3; // ������ ���� ��

    [Tooltip("���� ���� ���� ������ (��).")]
    [SerializeField] private float spawnInterval = 0.5f; // ���� ���� �� ������

    [Tooltip("���Ͱ� ������ �ִ� ���� (������ �߽����� ������).")]
    [SerializeField] private float spawnRadius = 5f; // ���Ͱ� ������ ���� ������

    [Tooltip("�÷��̾ �����ʸ� Ʈ������ ����Դϴ�. �±׸� 'Player'�� �����ϼ���.")]
    [SerializeField] private GameObject player; // �÷��̾� ������Ʈ (Inspector���� �Ҵ� �Ǵ� "Player" �±׷� ã��)

    [Tooltip("������ Ʈ���ſ� ���� �� ���� ���� ���۱����� ������ (��).")]
    [SerializeField] private float triggerDelay = 1.0f; // �÷��̾� ���� �� ���� ���۱����� ������

    [Header("Debugging & State")]
    [SerializeField] private bool hasSpawned = false; // �̹� ������ �Ϸ�Ǿ����� ����
    private List<GameObject> spawnedMonsters = new List<GameObject>(); // ������ ���͵��� �����ϱ� ���� ����Ʈ

    void Start()
    {
        // �÷��̾� ������Ʈ�� �Ҵ���� �ʾҴٸ�, "Player" �±׸� ���� ������Ʈ�� ã��
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("MonsterSpawner: Player GameObject not found. Please assign it in the Inspector or tag your player with 'Player'.");
            }
        }

        // ������ ���� �������� �Ҵ���� �ʾҴٸ� ���
        if (monsterPrefab == null)
        {
            Debug.LogError("MonsterSpawner: Monster Prefab is not assigned! This spawner cannot function.", this);
            this.enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
        }

        // ������ ������Ʈ�� Collider�� ���ų� Is Trigger�� Ȱ��ȭ���� �ʾҴٸ� ���
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("MonsterSpawner: This spawner needs a Collider component with 'Is Trigger' enabled to detect player entry.", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ������ ������ ���԰�, ���� �������� �ʾҴٸ�
        if (other.gameObject == player && !hasSpawned)
        {
            Debug.Log($"�÷��̾� '{player.name}'�� ������ '{gameObject.name}' ������ �����߽��ϴ�. {triggerDelay}�� �� ���� ���� ����.");
            // ���� ������ �� ���� ���� �ڷ�ƾ ����
            Invoke("StartSpawningCoroutine", triggerDelay);
        }
    }

    void StartSpawningCoroutine()
    {
        if (!hasSpawned) // �ߺ� ȣ�� ����
        {
            StartCoroutine(SpawnMonsters());
            hasSpawned = true; // ���� ���� �÷��� ����
        }
    }

    IEnumerator SpawnMonsters()
    {
        Debug.Log($"���� ���� ����! {numberOfMonstersToSpawn} ���� ���� ����.");

        // (���� ����) DungeonManager�� �ִٸ� �� ���� �� ������Ʈ
        // �� �����ʰ� ������ ������ �����ʶ�� ���⼭ totalMonstersInDungeon�� ����

        ////if (DungeonManager.Instance != null)
        ////{
        ////    DungeonManager.Instance.SetTotalMonsters(numberOfMonstersToSpawn);
        ////}


        for (int i = 0; i < numberOfMonstersToSpawn; i++)
        {
            // ���� ��ġ ��� (������ �ֺ� spawnRadius ���� ���� ��ġ)
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius; // ���� ���� �� ���� ����
            randomOffset.y = 0; // Y���� 0���� �����Ͽ� �ٴڿ� ���� (�ʿ信 ���� Y�� ������ �߰� ����)
            Vector3 spawnPosition = transform.position + randomOffset;

            // ���� ���̿� ���߱� (���� ����, Terrain�̳� NavMesh ��� ��)
            // Raycast�� �Ʒ��� ���� ������ ��� ��ġ�� ã�ų�, NavMesh.SamplePosition ���
            // ��:
            /*
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, 100f, LayerMask.GetMask("Ground")))
            {
                spawnPosition.y = hit.point.y + 0.1f; // ���� �ణ ��
            }
            */
            // �Ǵ� NavMesh ��� ��:
            /*
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPosition, out navHit, spawnRadius, UnityEngine.AI.NavMesh.AllAreas))
            {
                spawnPosition = navHit.position;
            }
            */

            // ���� �ν��Ͻ�ȭ (����)
            GameObject spawnedMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
            spawnedMonsters.Add(spawnedMonster); // ������ ���� ����Ʈ�� �߰�

            Debug.Log($"���� '{spawnedMonster.name}' ������. (��ġ: {spawnPosition})");

            // ���� ���� �������� ������
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("��� ���� ���� �Ϸ�!");
        // ���� �Ϸ� �� ������ ��Ȱ��ȭ (���� ����, �ʿ信 ����)
        // gameObject.SetActive(false);
    }

    // ����Ƽ �����Ϳ��� ���� ���� �ð�ȭ
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // �������� ��ġ�� ���̾� ���Ǿ� �׸���
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    // (�ɼ�) ������ ���͵��� �ܺο��� ������ �ʿ䰡 �ִٸ�
    public List<GameObject> GetSpawnedMonsters()
    {
        return spawnedMonsters;
    }
}