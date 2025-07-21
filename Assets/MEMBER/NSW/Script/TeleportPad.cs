using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필요

public class TeleportPad : MonoBehaviour
{
    [Header("Teleport Pad Settings")]
    [Tooltip("이 텔레포트 패드의 고유 ID (선택 사항, 내부적으로만 사용).")]
    public string padID = "TeleportPad_A";

    [Header("Destination Settings")]
    [Tooltip("플레이어가 이동할 목적지 Transform을 할당하세요.")]
    public Transform destinationTransform; // 플레이어가 이동할 목적지 위치 및 회전

    [Tooltip("목적지 트랜스폼의 앞쪽으로 플레이어를 내보낼 거리.")]
    public float spawnOffset = 1.5f; // 목적지 트랜스폼의 앞쪽으로 내보낼 거리

    [Header("Player Settings")]
    public GameObject player; // 플레이어 오브젝트 (Inspector에서 할당 또는 "Player" 태그로 찾음)
    public float teleportDelay = 0.5f; // 플레이어가 패드에 들어간 후 이동까지의 딜레이
    public float fadeDuration = 0.5f; // 화면 페이드 인/아웃 시간 (옵션, 실제 구현은 별도 스크립트 권장)

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
                Debug.LogError("TeleportPad: Player GameObject not found. Please assign it in the Inspector or tag your player with 'Player'.");
            }
        }

        // 텔레포트 패드 오브젝트에 Collider가 없으면 경고
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("TeleportPad: This TeleportPad needs a Collider component with 'Is Trigger' enabled to detect player entry.", this);
        }

        // 목적지 Transform이 설정되지 않았으면 경고
        if (destinationTransform == null)
        {
            Debug.LogError($"TeleportPad '{padID}': Destination Transform is not set. This pad cannot function!", this);
            this.enabled = false; // 스크립트 비활성화
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
                Debug.Log($"플레이어 '{player.name}'가 텔레포트 패드 '{padID}'에 진입했습니다. 이동 시도.");
                StartCoroutine(PerformTeleport());
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
                Debug.Log($"플레이어 '{player.name}'가 텔레포트 패드 '{padID}'에서 벗어났습니다.");
            }
        }
    }

    IEnumerator PerformTeleport()
    {
        // 1. 텔레포트 딜레이
        yield return new WaitForSeconds(teleportDelay);

        // 2. 화면 페이드 아웃 (옵션)
        // 실제 게임에서는 ScreenFader 같은 전역 스크립트를 사용합니다.
        // 예: ScreenFader.Instance.FadeOut(fadeDuration);
        Debug.Log("화면 페이드 아웃 중...");
        yield return new WaitForSeconds(fadeDuration); // 페이드 아웃이 완료될 때까지 기다림 (임시)

        // 플레이어 움직임 비활성화 (순간 이동 중 이동 방지)
        SetPlayerMovement(false);

        // 3. 플레이어 위치 및 회전 설정
        if (destinationTransform != null)
        {
            Vector3 destination = destinationTransform.position + destinationTransform.forward * spawnOffset;
            player.transform.position = destination;
            player.transform.rotation = destinationTransform.rotation;
            Debug.Log($"플레이어를 '{destinationTransform.name}' (Pad ID: {padID}) 위치로 이동시켰습니다: {destination}");
        }
        else
        {
            Debug.LogError($"TeleportPad '{padID}': 목적지 Transform이 Null입니다. 이동 실패!");
        }

        // 4. 이동 완료 후 페이드 인 및 플래그 리셋
        StartCoroutine(FinishTeleportation());
    }

    IEnumerator FinishTeleportation()
    {
        // 1. 화면 페이드 인 (옵션)
        // 예: ScreenFader.Instance.FadeIn(fadeDuration);
        Debug.Log("화면 페이드 인 중...");
        yield return new WaitForSeconds(fadeDuration); // 페이드 인이 완료될 때까지 기다림 (임시)

        // 2. 플레이어 움직임 활성화
        SetPlayerMovement(true);

        // 3. 텔레포트 플래그 리셋
        isTeleporting = false;
        playerInRange = false;
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

    // 유니티 에디터에서 텔레포트 패드와 목적지를 시각화
    void OnDrawGizmos()
    {
        // 현재 패드 범위 표시
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        // 목적지 트랜스폼이 할당되어 있다면 목적지 표시
        if (destinationTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(destinationTransform.position, 0.5f); // 목적지 지점
            Gizmos.DrawRay(destinationTransform.position, destinationTransform.forward * 2f); // 목적지 전방향
            Gizmos.DrawSphere(destinationTransform.position + destinationTransform.forward * spawnOffset, 0.3f); // 스폰 지점

            // 현재 패드와 목적지를 연결하는 선
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destinationTransform.position);
        }
    }
}