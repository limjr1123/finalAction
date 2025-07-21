using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ �� �ʿ�

public class PlayerRespawnManager : MonoBehaviour
{
    [Header("Respawn Settings")]
    public GameObject playerPrefab; // �÷��̾� ������ (�÷��̾� ������Ʈ�� ���ų� ���� �ٲ� �� �ν��Ͻ�ȭ)
    public float respawnDelay = 3f; // ��� �� ������������ ������
    public string initialSceneName = "GameScene"; // ����� ������ ���� ��� ������ �⺻ ��

    private GameObject currentPlayerInstance;
    private PlayerHealth currentPlayerHealth;

    void Awake()
    {
        // �� ��ȯ �ÿ��� �ı����� �ʵ��� ���� (���� ����, �ʿ信 ����)
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        //PlayerHealth�� ��� �̺�Ʈ ����
        PlayerHealth.OnPlayerDied += HandlePlayerDied;
    }

    void OnDisable()
    {
        // ��ũ��Ʈ ��Ȱ��ȭ �� ���� ���� (�޸� ���� ����)
        PlayerHealth.OnPlayerDied -= HandlePlayerDied;
    }

    void Start()
    {
        // �� ���� �� �÷��̾� �ν��Ͻ� ���� ��������
        FindAndSetPlayerReference();
    }

    // �÷��̾� ��� �̺�Ʈ �߻� �� ȣ��� �Լ�
    void HandlePlayerDied()
    {
        Debug.Log("RespawnManager: �÷��̾� ��� ����. " + respawnDelay + "�� �� ������ �õ�.");
        // ��� UI ǥ�� �� �߰� ȿ��
        Invoke("RespawnPlayer", respawnDelay);
    }

    // �÷��̾� ������ ����
    void RespawnPlayer()
    {
        Debug.Log("RespawnManager: ������ ���� ����.");

        // 1. ���������� ����� �� �̸� ��������
        string lastSceneName = PlayerPrefs.GetString("LastSceneName", "");

        if (string.IsNullOrEmpty(lastSceneName))
        {
            Debug.LogWarning("����� ���� �����Ͱ� �����ϴ�. ���ο� ������ �����մϴ�.");
            // ����� �����Ͱ� ������ ó�� ������ �ε� (Ȥ�� Ÿ��Ʋ ȭ������)
            SceneManager.LoadScene(initialSceneName);
            return;
        }

        // 2. ���� ���� ����� ���� �ٸ� ��� �� �ε�
        if (SceneManager.GetActiveScene().name != lastSceneName)
        {
            Debug.Log($"RespawnManager: �� ��ȯ �ʿ�. '{SceneManager.GetActiveScene().name}' -> '{lastSceneName}'");
            // �� �ε尡 �Ϸ�Ǹ� ApplySavedPlayerPosition �Լ� ȣ���ϵ��� �̺�Ʈ ����
            SceneManager.sceneLoaded += OnSceneLoadedAfterRespawn;
            SceneManager.LoadScene(lastSceneName);
        }
        else // ���� ���� ����� ���� ������ ���
        {
            //Debug("RespawnManager: ���� �� ������ ������.");
            ApplySavedPlayerPosition();
        }
    }

    // �� �ε� �Ϸ� �� ȣ��� �Լ� (���� ��ȯ�Ǿ��� ��)
    void OnSceneLoadedAfterRespawn(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("RespawnManager: �� �ε� �Ϸ� �̺�Ʈ �߻�.");
        // �÷��̾� ������Ʈ ���� �ٽ� ���� (���� �ٲ�����Ƿ�)
        FindAndSetPlayerReference();
        // �÷��̾� ��ġ ����
        ApplySavedPlayerPosition();
        // �̺�Ʈ �ڵ鷯 ���� (�ߺ� ȣ�� ����)
        SceneManager.sceneLoaded -= OnSceneLoadedAfterRespawn;
    }

    // ����� �÷��̾� ��ġ�� �̵���Ű�� Ȱ��ȭ�ϴ� �Լ�
    void ApplySavedPlayerPosition()
    {
        if (!PlayerPrefs.HasKey("PlayerPosX"))
        {
            Debug.LogWarning("����� �÷��̾� ��ġ �����Ͱ� �����ϴ�. �÷��̾ �⺻ ��ġ�� ��ġ�մϴ�.");
            // �⺻ ��ġ�� �÷��̾� Ȱ��ȭ�� �ϰų�, Ư�� ���� ��ġ�� �̵�
            if (currentPlayerInstance != null)
            {
                //currentPlayerHealth.Respawn(); // ü�� ȸ�� �� Ȱ��ȭ
            }
            return;
        }

        float posX = PlayerPrefs.GetFloat("PlayerPosX");
        float posY = PlayerPrefs.GetFloat("PlayerPosY");
        float posZ = PlayerPrefs.GetFloat("PlayerPosZ");
        float rotY = PlayerPrefs.GetFloat("PlayerRotY");

        Vector3 savedPosition = new Vector3(posX, posY, posZ);
        Quaternion savedRotation = Quaternion.Euler(0, rotY, 0);

        if (currentPlayerInstance == null)
        {
            Debug.Log("RespawnManager: ���� ���� �÷��̾� �ν��Ͻ��� �����ϴ�. ���������� �����մϴ�.");
            if (playerPrefab != null)
            {
                currentPlayerInstance = Instantiate(playerPrefab, savedPosition, savedRotation);
                currentPlayerInstance.tag = "Player"; // �±� ���� (�߿�!)
                currentPlayerHealth = currentPlayerInstance.GetComponent<PlayerHealth>();
                if (currentPlayerHealth == null)
                {
                    Debug.LogError("�÷��̾� �����տ� PlayerHealth ��ũ��Ʈ�� �����ϴ�!");
                }
            }
            else
            {
                Debug.LogError("�÷��̾� �������� PlayerRespawnManager ��ũ��Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
                return;
            }
        }
        else
        {
            // ���� �÷��̾� ������Ʈ�� �ִٸ� ��ġ/ȸ���� ����
            currentPlayerInstance.transform.position = savedPosition;
            currentPlayerInstance.transform.rotation = savedRotation;
            Debug.Log($"�÷��̾ ����� ��ġ ({savedPosition})�� �̵����׽��ϴ�.");
        }

        // �÷��̾� ü�� ȸ�� �� ������Ʈ Ȱ��ȭ
        //currentPlayerHealth?.Respawn();
    }

    // ���� ������ �÷��̾� ������Ʈ�� ã�� ������ �����ϴ� �Լ�
    void FindAndSetPlayerReference()
    {
        currentPlayerInstance = GameObject.FindGameObjectWithTag("Player");
        if (currentPlayerInstance != null)
        {
            currentPlayerHealth = currentPlayerInstance.GetComponent<PlayerHealth>();
            if (currentPlayerHealth == null)
            {
                Debug.LogError("�÷��̾� ������Ʈ�� PlayerHealth ��ũ��Ʈ�� �����ϴ�! ������ ����� ����� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("���� 'Player' �±׸� ���� ������Ʈ�� �����ϴ�. ������ �� �������� �ν��Ͻ�ȭ�� ���Դϴ�.");
        }
    }

    // �׽�Ʈ��: F2 Ű�� ���� ��� (������)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("���� ��� �׽�Ʈ!");
            currentPlayerHealth?.TakeDamage(currentPlayerHealth.maxHealth); // ������ ü�� 0 �����
        }
    }
}