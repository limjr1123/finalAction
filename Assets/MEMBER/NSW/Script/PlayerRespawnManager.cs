using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환 시 필요

public class PlayerRespawnManager : MonoBehaviour
{
    [Header("Respawn Settings")]
    public GameObject playerPrefab; // 플레이어 프리팹 (플레이어 오브젝트가 없거나 씬이 바뀔 때 인스턴스화)
    public float respawnDelay = 3f; // 사망 후 리스폰까지의 딜레이
    public string initialSceneName = "GameScene"; // 저장된 게임이 없을 경우 시작할 기본 씬

    private GameObject currentPlayerInstance;
    private PlayerHealth currentPlayerHealth;

    void Awake()
    {
        // 씬 전환 시에도 파괴되지 않도록 설정 (선택 사항, 필요에 따라)
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        //PlayerHealth의 사망 이벤트 구독
        PlayerHealth.OnPlayerDied += HandlePlayerDied;
    }

    void OnDisable()
    {
        // 스크립트 비활성화 시 구독 해제 (메모리 누수 방지)
        PlayerHealth.OnPlayerDied -= HandlePlayerDied;
    }

    void Start()
    {
        // 씬 시작 시 플레이어 인스턴스 참조 가져오기
        FindAndSetPlayerReference();
    }

    // 플레이어 사망 이벤트 발생 시 호출될 함수
    void HandlePlayerDied()
    {
        Debug.Log("RespawnManager: 플레이어 사망 감지. " + respawnDelay + "초 후 리스폰 시도.");
        // 사망 UI 표시 등 추가 효과
        Invoke("RespawnPlayer", respawnDelay);
    }

    // 플레이어 리스폰 로직
    void RespawnPlayer()
    {
        Debug.Log("RespawnManager: 리스폰 로직 시작.");

        // 1. 마지막으로 저장된 씬 이름 가져오기
        string lastSceneName = PlayerPrefs.GetString("LastSceneName", "");

        if (string.IsNullOrEmpty(lastSceneName))
        {
            Debug.LogWarning("저장된 게임 데이터가 없습니다. 새로운 게임을 시작합니다.");
            // 저장된 데이터가 없으면 처음 씬으로 로드 (혹은 타이틀 화면으로)
            SceneManager.LoadScene(initialSceneName);
            return;
        }

        // 2. 현재 씬과 저장된 씬이 다른 경우 씬 로드
        if (SceneManager.GetActiveScene().name != lastSceneName)
        {
            Debug.Log($"RespawnManager: 씬 전환 필요. '{SceneManager.GetActiveScene().name}' -> '{lastSceneName}'");
            // 씬 로드가 완료되면 ApplySavedPlayerPosition 함수 호출하도록 이벤트 연결
            SceneManager.sceneLoaded += OnSceneLoadedAfterRespawn;
            SceneManager.LoadScene(lastSceneName);
        }
        else // 현재 씬이 저장된 씬과 동일한 경우
        {
            //Debug("RespawnManager: 동일 씬 내에서 리스폰.");
            ApplySavedPlayerPosition();
        }
    }

    // 씬 로드 완료 후 호출될 함수 (씬이 전환되었을 때)
    void OnSceneLoadedAfterRespawn(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("RespawnManager: 씬 로드 완료 이벤트 발생.");
        // 플레이어 오브젝트 참조 다시 설정 (씬이 바뀌었으므로)
        FindAndSetPlayerReference();
        // 플레이어 위치 적용
        ApplySavedPlayerPosition();
        // 이벤트 핸들러 제거 (중복 호출 방지)
        SceneManager.sceneLoaded -= OnSceneLoadedAfterRespawn;
    }

    // 저장된 플레이어 위치로 이동시키고 활성화하는 함수
    void ApplySavedPlayerPosition()
    {
        if (!PlayerPrefs.HasKey("PlayerPosX"))
        {
            Debug.LogWarning("저장된 플레이어 위치 데이터가 없습니다. 플레이어를 기본 위치에 배치합니다.");
            // 기본 위치에 플레이어 활성화만 하거나, 특정 시작 위치로 이동
            if (currentPlayerInstance != null)
            {
                //currentPlayerHealth.Respawn(); // 체력 회복 및 활성화
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
            Debug.Log("RespawnManager: 현재 씬에 플레이어 인스턴스가 없습니다. 프리팹으로 생성합니다.");
            if (playerPrefab != null)
            {
                currentPlayerInstance = Instantiate(playerPrefab, savedPosition, savedRotation);
                currentPlayerInstance.tag = "Player"; // 태그 설정 (중요!)
                currentPlayerHealth = currentPlayerInstance.GetComponent<PlayerHealth>();
                if (currentPlayerHealth == null)
                {
                    Debug.LogError("플레이어 프리팹에 PlayerHealth 스크립트가 없습니다!");
                }
            }
            else
            {
                Debug.LogError("플레이어 프리팹이 PlayerRespawnManager 스크립트에 할당되지 않았습니다!");
                return;
            }
        }
        else
        {
            // 기존 플레이어 오브젝트가 있다면 위치/회전만 변경
            currentPlayerInstance.transform.position = savedPosition;
            currentPlayerInstance.transform.rotation = savedRotation;
            Debug.Log($"플레이어를 저장된 위치 ({savedPosition})로 이동시켰습니다.");
        }

        // 플레이어 체력 회복 및 오브젝트 활성화
        //currentPlayerHealth?.Respawn();
    }

    // 현재 씬에서 플레이어 오브젝트를 찾아 참조를 설정하는 함수
    void FindAndSetPlayerReference()
    {
        currentPlayerInstance = GameObject.FindGameObjectWithTag("Player");
        if (currentPlayerInstance != null)
        {
            currentPlayerHealth = currentPlayerInstance.GetComponent<PlayerHealth>();
            if (currentPlayerHealth == null)
            {
                Debug.LogError("플레이어 오브젝트에 PlayerHealth 스크립트가 없습니다! 리스폰 기능을 사용할 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("씬에 'Player' 태그를 가진 오브젝트가 없습니다. 리스폰 시 프리팹을 인스턴스화할 것입니다.");
        }
    }

    // 테스트용: F2 키로 강제 사망 (디버깅용)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("강제 사망 테스트!");
            currentPlayerHealth?.TakeDamage(currentPlayerHealth.maxHealth); // 강제로 체력 0 만들기
        }
    }
}