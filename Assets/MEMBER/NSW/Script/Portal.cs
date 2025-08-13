using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string portalID = "DefaultPortal";

    [Header("Scene Transition")]
    [Tooltip("이 포탈을 통해 이동할 씬의 이름 (필수). Build Settings에 추가되어 있어야 합니다.")]
    public string targetSceneName;
    [Tooltip("이 포탈을 통해 이동했을 때, 도착할 씬에 있는 목적지 포탈의 ID입니다.")]
    public string targetPortalID;

    [Header("Cooldown Settings")]
    [Tooltip("포탈 사용 후 다시 사용하기까지의 대기 시간(초)입니다.")]
    public float portalCooldown = 3.0f;

    private bool isTeleporting = false;
    private static float lastTeleportTime = -1f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            if (Time.time < lastTeleportTime + portalCooldown)
            {
                Debug.Log($"포탈 쿨타임 중입니다. {((lastTeleportTime + portalCooldown) - Time.time):F1}초 남음.");
                return;
            }

            isTeleporting = true;
            Debug.Log($"플레이어 '{other.name}'가 포탈 '{portalID}'에 진입했습니다. 씬 '{targetSceneName}'의 '{targetPortalID}'로 이동 시도.");
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    IEnumerator TeleportPlayer(GameObject player)
    {
        // 쿨타임 설정
        lastTeleportTime = Time.time;

        // 화면 페이드 아웃 등의 효과를 여기에 추가할 수 있습니다.
        // 이 부분은 SceneLoader의 로딩 UI가 처리하므로, 여기서는 딜레이를 주지 않거나
        // 페이드 효과를 SceneLoader와 연동하는 것이 좋습니다.
        // 여기서는 일단 로그만 남깁니다.
        Debug.Log("씬 로딩을 시작합니다...");

        // 플레이어 이동 비활성화
        SetPlayerMovement(player, false);

        // PlayerPrefs를 사용해 도착지 포탈 ID를 다음 씬으로 전달
        PlayerPrefs.SetString("TargetPortalIDAfterSceneLoad", targetPortalID);
        PlayerPrefs.Save();

        // 씬 로드 완료 시 플레이어 위치를 재설정할 함수를 등록
        SceneManager.sceneLoaded += OnSceneLoadedAndTeleport;

        // --- 중요 변경점 ---
        // 기존의 동기 방식 로드 대신, 로딩 UI를 표시하는 비동기 방식 SceneLoader를 호출합니다.
        SceneLoader.LoadSceneAsync(targetSceneName);

        // SceneLoader가 씬 로딩을 시작했으므로 이 코루틴의 역할은 끝났습니다.
        yield break;
    }

    void OnSceneLoadedAndTeleport(Scene scene, LoadSceneMode mode)
    {
        // 이벤트 중복 호출을 막기 위해 즉시 등록 해제
        SceneManager.sceneLoaded -= OnSceneLoadedAndTeleport;

        // PlayerManager나 다른 수단을 통해 플레이어 오브젝트를 찾습니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("새로운 씬에서 'Player'를 찾을 수 없습니다!");
            isTeleporting = false;
            return;
        }

        string storedTargetPortalID = PlayerPrefs.GetString("TargetPortalIDAfterSceneLoad", "");
        if (string.IsNullOrEmpty(storedTargetPortalID))
        {
            Debug.LogError("PlayerPrefs에서 TargetPortalID를 찾을 수 없습니다!");
            FinishTeleportation(player);
            return;
        }

        Portal targetPortal = FindTargetPortal(storedTargetPortalID);

        if (targetPortal != null)
        {
            // 플레이어를 목적지 포탈의 위치와 방향으로 설정
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
        FinishTeleportation(player);
    }

    Portal FindTargetPortal(string portalID)
    {
        Portal[] allPortals = FindObjectsOfType<Portal>();
        foreach (Portal p in allPortals)
        {
            if (p.portalID == portalID)
            {
                return p;
            }
        }
        return null;
    }

    void FinishTeleportation(GameObject player)
    {
        // SceneLoader의 로딩 UI가 사라진 후 플레이어 이동을 활성화합니다.
        Debug.Log("화면 전환 완료. 플레이어 이동을 다시 활성화합니다.");
        SetPlayerMovement(player, true);
        isTeleporting = false;
        Debug.Log("텔레포트 완료.");
    }

    void SetPlayerMovement(GameObject player, bool enabled)
    {
        if (player != null)
        {
            // 실제 프로젝트에 맞는 플레이어 이동 제어 스크립트를 활성화/비활성화 하세요.
            // 예: player.GetComponent<PlayerController>().enabled = enabled;
            Debug.Log($"플레이어 움직임 {(enabled ? "활성화" : "비활성화")}.");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}