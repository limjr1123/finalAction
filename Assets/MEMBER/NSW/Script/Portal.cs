using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // 코루틴 사용을 위해 필요

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string portalID = "DefaultPortal"; // 이 포탈의 고유 ID (씬 내에서 다른 포탈이 이 포탈을 목적지로 삼을 때 사용)

    [Header("Scene Transition")]
    [Tooltip("이 포탈을 통해 이동할 씬의 이름 (필수). Build Settings에 추가되어 있어야 합니다.")]
    public string targetSceneName; // 이동할 씬 이름 (필수)
    [Tooltip("이 포탈을 통해 이동했을 때, 도착할 씬에 있는 목적지 포탈의 ID입니다.")]
    public string targetPortalID; // 도착할 씬에 있는 목적지 포탈의 고유 ID

    [Header("Player Settings")]
    public GameObject player; // 플레이어 오브젝트 (Inspector에서 할당 또는 "Player" 태그로 찾음)
    public float teleportDelay = 0.5f; // 플레이어가 포탈에 들어간 후 이동까지의 딜레이
    public float fadeDuration = 0.5f; // 화면 페이드 인/아웃 시간 (실제 구현은 별도 스크립트 권장)

    private bool playerInRange = false;
    private bool isTeleporting = false; // 중복 텔레포트 방지 플래그

    void Start()
    {
        // 플레이어 오브젝트가 할당되지 않았다면, "Player" 태그를 가진 오브젝트를 찾음
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Portal: Player GameObject not found. Please assign it in the Inspector or tag your player with 'Player'.");
            }
        }

        // 포탈 오브젝트에 Collider가 없으면 경고
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("Portal: This Portal needs a Collider component with 'Is Trigger' enabled to detect player entry.", this);
        }

        // 목적지 씬 이름이 설정되지 않았으면 경고 (씬 이동 전용이므로 필수)
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError($"Portal '{portalID}': Target Scene Name is not set. This portal cannot function without a target scene!", this);
        }
        // 목적지 포탈 ID가 설정되지 않았으면 경고 (씬 로드 후 플레이어를 배치할 때 필요)
        if (string.IsNullOrEmpty(targetPortalID))
        {
            Debug.LogWarning($"Portal '{portalID}': Target Portal ID is not set. Player might not spawn at the correct location in the new scene.", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && !isTeleporting)
        {
            if (!playerInRange) // 중복 트리거 방지
            {
                playerInRange = true;
                isTeleporting = true; // 텔레포트 시작 플래그 설정
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
            // 플레이어가 나갔을 때 텔레포트가 진행 중이지 않다면 플래그 리셋
            if (!isTeleporting)
            {
                Debug.Log($"플레이어 '{player.name}'가 포탈 '{portalID}'에서 벗어났습니다.");
            }
        }
    }

    IEnumerator TeleportPlayer()
    {
        // 1. 텔레포트 딜레이
        yield return new WaitForSeconds(teleportDelay);

        // 2. 화면 페이드 아웃 (옵션)
        // 실제 게임에서는 ScreenFader 같은 전역 스크립트를 사용합니다.
        // 예를 들어 ScreenFader.Instance.FadeOut(fadeDuration);
        Debug.Log("화면 페이드 아웃 중...");
        yield return new WaitForSeconds(fadeDuration); // 페이드 아웃이 완료될 때까지 기다림 (임시)

        // 플레이어 움직임 비활성화 (순간 이동 중 이동 방지)
        SetPlayerMovement(false);

        // 3. 목적지 정보를 PlayerPrefs에 저장
        // 씬 로드 후 플레이어를 올바른 위치에 배치하기 위해 필요
        PlayerPrefs.SetString("TargetPortalIDAfterSceneLoad", targetPortalID);
        PlayerPrefs.Save();

        // 4. 씬 로드
        // 씬 로드가 완료되면 OnSceneLoadedAndTeleport 함수 호출하도록 이벤트 연결
        SceneManager.sceneLoaded += OnSceneLoadedAndTeleport;
        SceneManager.LoadScene(targetSceneName);

        // isTeleporting 플래그는 이동 완료 후 OnSceneLoadedAndTeleport 함수에서 리셋
    }

    // 다른 씬 로드 완료 후 호출될 함수
    void OnSceneLoadedAndTeleport(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 '{scene.name}' 로드 완료. 텔레포트 목적지 찾기.");
        // 이벤트 핸들러 해제 (중복 호출 방지)
        SceneManager.sceneLoaded -= OnSceneLoadedAndTeleport;

        // PlayerPrefs에서 목적지 포탈 ID 다시 가져오기
        string storedTargetPortalID = PlayerPrefs.GetString("TargetPortalIDAfterSceneLoad", "");
        if (string.IsNullOrEmpty(storedTargetPortalID))
        {
            Debug.LogError("씬 로드 후 TargetPortalIDAfterSceneLoad 데이터가 PlayerPrefs에 없습니다! 플레이어를 배치할 수 없습니다.");
            FinishTeleportation();
            return;
        }

        // 새로 로드된 씬에서 플레이어 참조 다시 가져오기
        // 플레이어 오브젝트가 DontDestroyOnLoad 되지 않았다면, 새로운 씬에서 다시 찾아야 함.
        // 또는 플레이어 프리팹을 인스턴스화 해야 함 (PlayerRespawnManager의 로직과 유사)
        if (player == null || !player.activeInHierarchy) // 현재 씬에 플레이어가 없거나 비활성화되어 있다면
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                // 이 부분은 PlayerRespawnManager가 플레이어 프리팹을 인스턴스화하는 로직을 담당하는 것이 더 좋음.
                // 여기서는 플레이어가 씬에 존재한다고 가정하거나, GameManager가 플레이어를 관리한다고 가정.
                Debug.LogError("새로운 씬에서 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다! 플레이어 배치 실패.");
                FinishTeleportation();
                return;
            }
        }
        // 플레이어 오브젝트가 비활성화되어 있었다면 활성화
        player.SetActive(true);


        // 로드된 씬에서 목적지 포탈 찾기
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
            // 플레이어 위치 및 회전 설정 (포탈의 바로 앞)
            Vector3 destination = targetPortal.transform.position + targetPortal.transform.forward * 1.5f; // 포탈 앞 1.5 유닛
            player.transform.position = destination;
            player.transform.rotation = targetPortal.transform.rotation;
            Debug.Log($"플레이어를 새로운 씬의 '{targetPortal.portalID}' 포탈 위치로 이동시켰습니다: {destination}");
        }
        else
        {
            Debug.LogError($"새로운 씬 '{scene.name}'에서 목적지 포탈 ID '{storedTargetPortalID}'를 찾을 수 없습니다! 플레이어 배치 실패.");
        }

        // PlayerPrefs 데이터 삭제 (목적지 정보는 한 번 사용 후 삭제)
        PlayerPrefs.DeleteKey("TargetPortalIDAfterSceneLoad");
        PlayerPrefs.Save();

        // 이동 완료 후 플래그 리셋 및 플레이어 활성화
        FinishTeleportation();
    }

    void FinishTeleportation()
    {
        // 1. 화면 페이드 인 (옵션)
        // 예를 들어 ScreenFader.Instance.FadeIn(fadeDuration);
        Debug.Log("화면 페이드 인 중...");
        // yield return new WaitForSeconds(fadeDuration); // 실제로는 페이드 인이 완료될 때까지 기다림

        // 2. 플레이어 움직임 활성화
        SetPlayerMovement(true);

        // 3. 텔레포트 플래그 리셋
        isTeleporting = false;
        playerInRange = false; // OnTriggerExit 없이도 다시 들어갈 수 있도록
        Debug.Log("텔레포트 완료.");
    }

    void SetPlayerMovement(bool enabled)
    {
        if (player != null)
        {
            // 플레이어 움직임을 제어하는 스크립트가 있다면 해당 스크립트의 활성화를 조절
            // 예:
            // var controller = player.GetComponent<PlayerController>(); // 여러분의 플레이어 컨트롤러 스크립트
            // if (controller != null) controller.enabled = enabled;

            // 또는 Rigidbody를 사용하는 경우
            // var rb = player.GetComponent<Rigidbody>();
            // if (rb != null) rb.isKinematic = !enabled; // Kinematic으로 설정하면 물리적 이동 불가

            Debug.Log($"플레이어 움직임 {(enabled ? "활성화" : "비활성화")}.");
        }
    }

    // 유니티 에디터에서 포탈의 전방향을 시각화
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale); // 포탈 범위
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * 2f); // 포탈 전방향 표시
        Gizmos.DrawSphere(transform.position + transform.forward * 2f, 0.2f); // 전방향 끝점
    }
}