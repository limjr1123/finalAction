using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerRespawnManager : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float respawnDelay = 3f; // 사망 후 리스폰까지의 딜레이
    public string initialSceneName = "TitleScene"; // 저장된 데이터가 없을 때 돌아갈 씬

    private GameObject currentPlayerInstance;
    // ▼▼▼ PlayerHealth 대신 PlayerStats 변수로 변경 ▼▼▼
    private PlayerStats currentPlayerStats;

    // 씬이 로드될 때마다 플레이어 참조를 다시 설정하기 위해 이벤트에 등록
    void OnEnable()
    {
        // ▼▼▼ PlayerHealth가 아닌 PlayerStats의 이벤트 구독 ▼▼▼
        PlayerStats.OnPlayerDied += HandlePlayerDied;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 오브젝트가 비활성화되거나 파괴될 때 이벤트 구독 해제 (메모리 누수 방지)
    void OnDisable()
    {
        // ▼▼▼ PlayerStats의 이벤트 구독 해제 ▼▼▼
        PlayerStats.OnPlayerDied -= HandlePlayerDied;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드 시마다 호출될 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 완전히 로드된 후, 다른 스크립트들이 정리될 시간을 주고 플레이어를 찾음
        StartCoroutine(DelayedSetup());
    }

    // 한 프레임 대기한 후 플레이어를 찾는 코루틴
    IEnumerator DelayedSetup()
    {
        // 한 프레임만 기다려서 다른 오브젝트들의 Awake()나 Start()가 실행될 시간을 줌
        yield return null;
        FindAndSetPlayerReference();
    }

    // 플레이어 사망 이벤트가 발생하면 호출될 함수
    void HandlePlayerDied()
    {
        Debug.Log("RespawnManager: 플레이어 사망 감지. " + respawnDelay + "초 후 리스폰을 시도합니다.");
        Invoke(nameof(RespawnPlayer), respawnDelay);
    }

    // 플레이어 리스폰 로직
    void RespawnPlayer()
    {
        Debug.Log("RespawnManager: 리스폰 로직을 시작합니다.");

        // 마지막으로 저장된 체크포인트나 씬 정보를 불러와서 해당 씬으로 이동
        // 이 부분은 게임의 저장/로드 시스템에 맞게 구현해야 합니다.
        // 여기서는 간단히 마지막 씬을 다시 로드한다고 가정합니다.
        string lastSceneName = PlayerPrefs.GetString("LastSceneName", SceneManager.GetActiveScene().name);

        if (string.IsNullOrEmpty(lastSceneName) || SceneManager.GetActiveScene().name != lastSceneName)
        {
            SceneManager.LoadScene(lastSceneName);
        }
        else
        {
            // 같은 씬에서 리스폰하는 경우, 바로 위치 적용
            ApplySavedPlayerPosition();
        }
    }

    // 저장된 플레이어 위치로 이동시키는 함수
    void ApplySavedPlayerPosition()
    {
        // 플레이어 참조가 유효한지 다시 한번 확인
        if (currentPlayerInstance == null)
        {
            FindAndSetPlayerReference();
            if (currentPlayerInstance == null)
            {
                Debug.LogError("리스폰 위치를 적용할 플레이어를 찾을 수 없습니다!");
                return;
            }
        }

        // PlayerPrefs나 다른 저장 시스템에서 마지막 위치를 가져옴
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float posX = PlayerPrefs.GetFloat("PlayerPosX");
            float posY = PlayerPrefs.GetFloat("PlayerPosY");
            float posZ = PlayerPrefs.GetFloat("PlayerPosZ");
            Vector3 savedPosition = new Vector3(posX, posY, posZ);

            currentPlayerInstance.transform.position = savedPosition;
            Debug.Log($"플레이어를 저장된 위치 ({savedPosition})로 이동시켰습니다.");
        }

        // PlayerStats에 체력을 회복하고 부활 상태로 만드는 함수가 있다면 호출
        // 예: currentPlayerStats.Respawn();
        // SendMessage는 해당 함수가 없어도 오류를 발생시키지 않아 안전합니다.
        currentPlayerStats?.SendMessage("Respawn", SendMessageOptions.DontRequireReceiver);
    }

    // 현재 씬에서 플레이어 오브젝트를 찾아 참조를 설정하는 핵심 함수
    void FindAndSetPlayerReference()
    {
        currentPlayerInstance = GameObject.FindGameObjectWithTag("Player");

        if (currentPlayerInstance != null)
        {
            // ▼▼▼ GetComponent<PlayerHealth> 대신 GetComponent<PlayerStats>로 변경 ▼▼▼
            currentPlayerStats = currentPlayerInstance.GetComponent<PlayerStats>();

            if (currentPlayerStats == null)
            {
                Debug.LogError("오류: 찾은 'Player' 태그 오브젝트에 PlayerStats 스크립트가 없습니다!");
            }
            else
            {
                Debug.Log("RespawnManager가 플레이어(" + currentPlayerInstance.name + ")의 참조를 성공적으로 설정했습니다.");
            }
        }
        else
        {
            Debug.LogWarning("경고: 씬에서 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }
}