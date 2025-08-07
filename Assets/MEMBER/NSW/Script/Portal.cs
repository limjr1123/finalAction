using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // 코루틴 사용을 위해 필요

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string portalID = "DefaultPortal";

    [Header("Scene Transition")]
    [Tooltip("이 포탈을 통해 이동할 씬의 이름 (필수). Build Settings에 추가되어 있어야 합니다.")]
    public string targetSceneName;
    [Tooltip("이 포탈을 통해 이동했을 때, 도착할 씬에 있는 목적지 포탈의 ID입니다.")]
    public string targetPortalID;

    [Header("Player Settings")]
    public GameObject player;
    public float teleportDelay = 0.5f;
    public float fadeDuration = 0.5f;

    [Header("Cooldown Settings")]
    [Tooltip("포탈 사용 후 다시 사용하기까지의 대기 시간(초)입니다.")]
    public float portalCooldown = 3.0f;

    // ▼▼▼ 오류가 발생한 변수들은 여기에 선언되어야 합니다 ▼▼▼
    private bool playerInRange = false;
    private bool isTeleporting = false; // 중복 텔레포트 방지 플래그
    // ▲▲▲ 이 부분의 누락 여부를 확인하세요 ▲▲▲

    private static float lastTeleportTime = -1f;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Portal: Player GameObject not found. Please assign it in the Inspector or tag your player with 'Player'.");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (Time.time < lastTeleportTime + portalCooldown)
        {
            Debug.Log($"포탈 쿨타임 중입니다. {((lastTeleportTime + portalCooldown) - Time.time):F1}초 남음.");
            return;
        }

        if (other.gameObject == player && !isTeleporting)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                isTeleporting = true;
                Debug.Log($"플레이어 '{player.name}'가 포탈 '{portalID}'에 진입했습니다. 씬 '{targetSceneName}'의 '{targetPortalID}'로 이동 시도.");
                StartCoroutine(TeleportPlayer());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
            if (!isTeleporting)
            {
                Debug.Log($"플레이어 '{player.name}'가 포탈 '{portalID}'에서 벗어났습니다.");
            }
        }
    }

    IEnumerator TeleportPlayer()
    {
        lastTeleportTime = Time.time;
        yield return new WaitForSeconds(teleportDelay);
        Debug.Log("화면 페이드 아웃 중...");
        yield return new WaitForSeconds(fadeDuration);
        SetPlayerMovement(false);
        PlayerPrefs.SetString("TargetPortalIDAfterSceneLoad", targetPortalID);
        PlayerPrefs.Save();
        SceneManager.sceneLoaded += OnSceneLoadedAndTeleport;
        SceneManager.LoadScene(targetSceneName);
    }

    // ... (이하 나머지 코드는 이전과 동일) ...

    void OnSceneLoadedAndTeleport(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 '{scene.name}' 로드 완료. 텔레포트 목적지 찾기.");
        SceneManager.sceneLoaded -= OnSceneLoadedAndTeleport;

        string storedTargetPortalID = PlayerPrefs.GetString("TargetPortalIDAfterSceneLoad", "");
        if (string.IsNullOrEmpty(storedTargetPortalID))
        {
            Debug.LogError("씬 로드 후 TargetPortalIDAfterSceneLoad 데이터가 PlayerPrefs에 없습니다! 플레이어를 배치할 수 없습니다.");
            FinishTeleportation();
            return;
        }

        if (player == null || !player.activeInHierarchy)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("새로운 씬에서 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다! 플레이어 배치 실패.");
                FinishTeleportation();
                return;
            }
        }
        player.SetActive(true);

        Portal[] allPortalsInNewScene = FindObjectsOfType<Portal>();
        Portal targetPortal = null;

        foreach (Portal p in allPortalsInNewScene)
        {
            if (p.portalID == storedTargetPortalID)
            {
                targetPortal = p;
                break;
            }
        }

        if (targetPortal != null)
        {
            Vector3 destination = targetPortal.transform.position + targetPortal.transform.forward * 1.5f;
            player.transform.position = destination;
            player.transform.rotation = targetPortal.transform.rotation;
            Debug.Log($"플레이어를 새로운 씬의 '{targetPortal.portalID}' 포탈 위치로 이동시켰습니다: {destination}");
        }
        else
        {
            Debug.LogError($"새로운 씬 '{scene.name}'에서 목적지 포탈 ID '{storedTargetPortalID}'를 찾을 수 없습니다! 플레이어 배치 실패.");
        }

        PlayerPrefs.DeleteKey("TargetPortalIDAfterSceneLoad");
        PlayerPrefs.Save();

        FinishTeleportation();
    }

    void FinishTeleportation()
    {
        Debug.Log("화면 페이드 인 중...");
        SetPlayerMovement(true);
        isTeleporting = false;
        playerInRange = false;
        Debug.Log("텔레포트 완료.");
    }

    void SetPlayerMovement(bool enabled)
    {
        if (player != null)
        {
            // 실제 프로젝트에 맞는 플레이어 이동 제어 코드를 여기에 추가하세요.
            Debug.Log($"플레이어 움직임 {(enabled ? "활성화" : "비활성화")}.");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
        Gizmos.DrawSphere(transform.position + transform.forward * 2f, 0.2f);
    }
}