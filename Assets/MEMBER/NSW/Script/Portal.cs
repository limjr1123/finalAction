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

    // isTeleporting 플래그는 여전히 중복 실행을 막기 위해 유용합니다.
    private bool isTeleporting = false;
    // lastTeleportTime을 static으로 유지하여 모든 포탈이 쿨타임을 공유하도록 합니다.
    private static float lastTeleportTime = -1f;

    void OnTriggerEnter(Collider other)
    {
        // 포탈에 들어온 오브젝트가 'Player' 태그를 가졌는지 확인합니다.
        // 이렇게 하면 특정 프리팹에 종속되지 않고, 태그만 맞으면 누구든 이동 가능합니다.
        if (other.CompareTag("Player") && !isTeleporting)
        {
            // 쿨타임 확인
            if (Time.time < lastTeleportTime + portalCooldown)
            {
                Debug.Log($"포탈 쿨타임 중입니다. {((lastTeleportTime + portalCooldown) - Time.time):F1}초 남음.");
                return;
            }

            isTeleporting = true;
            Debug.Log($"플레이어 '{other.name}'가 포탈 '{portalID}'에 진입했습니다. 씬 '{targetSceneName}'의 '{targetPortalID}'로 이동 시도.");

            // 이동할 플레이어 오브젝트를 코루틴에 전달합니다.
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    IEnumerator TeleportPlayer(GameObject player) // player 오브젝트를 매개변수로 받습니다.
    {
        // 쿨타임 설정
        lastTeleportTime = Time.time;

        // 화면 페이드 아웃 등의 효과를 여기에 추가할 수 있습니다.
        // 예: Fader.FadeOut(0.5f);
        Debug.Log("화면 페이드 아웃 시작...");
        yield return new WaitForSeconds(0.5f); // 페이드 시간 동안 대기

        // 플레이어 이동 비활성화
        SetPlayerMovement(player, false);

        // PlayerPrefs를 사용해 도착지 포탈 ID를 다음 씬으로 전달
        PlayerPrefs.SetString("TargetPortalIDAfterSceneLoad", targetPortalID);
        PlayerPrefs.Save();

        // 씬 로드 완료 시 호출될 함수를 등록
        SceneManager.sceneLoaded += OnSceneLoadedAndTeleport;

        // 목표 씬 로드
        SceneManager.LoadScene(targetSceneName);
    }

    void OnSceneLoadedAndTeleport(Scene scene, LoadSceneMode mode)
    {
        // 이벤트 중복 호출을 막기 위해 등록 해제
        SceneManager.sceneLoaded -= OnSceneLoadedAndTeleport;

        // DontDestroyOnLoad로 살아남은 플레이어를 태그로 다시 찾습니다.
        GameObject player = PlayerManager.instance.gameObject; // PlayerManager를 통해 플레이어를 찾는 것이 더 안정적입니다.
        // 또는 기존 방식대로 GameObject.FindGameObjectWithTag("Player"); 를 사용해도 됩니다.

        if (player == null)
        {
            Debug.LogError("새로운 씬에서 'Player'를 찾을 수 없습니다! PlayerManager가 플레이어에 제대로 적용되었는지 확인하세요.");
            isTeleporting = false; // 텔레포트 상태 초기화
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

        // 사용한 PlayerPrefs 데이터 삭제
        PlayerPrefs.DeleteKey("TargetPortalIDAfterSceneLoad");

        FinishTeleportation(player);
    }

    // 도착지 포탈을 찾는 로직을 별도 함수로 분리하면 코드가 깔끔해집니다.
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
        return null; // 못 찾았으면 null 반환
    }

    void FinishTeleportation(GameObject player)
    {
        // 화면 페이드 인 등의 효과
        // 예: Fader.FadeIn(0.5f);
        Debug.Log("화면 페이드 인 시작...");

        // 플레이어 이동 다시 활성화
        SetPlayerMovement(player, true);
        isTeleporting = false; // 텔레포트 상태 해제
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

    // 기즈모는 디버깅에 유용하므로 그대로 둡니다.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}