using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("카메라가 따라갈 플레이어 오브젝트를 할당하세요.")]
    public Transform target; // 카메라가 따라갈 플레이어 Transform (Inspector에서 할당)

    [Header("Follow Settings")]
    [Tooltip("플레이어로부터 카메라가 떨어질 거리입니다.")]
    public Vector3 offset = new Vector3(0f, 2f, -5f); // 플레이어로부터의 상대적인 위치 (Y축 2m 위, Z축 -5m 뒤)
    [Tooltip("카메라가 목표 위치에 도달하는 속도 (높을수록 빠르게 따라감).")]
    public float smoothSpeed = 0.125f; // 카메라 움직임의 부드러움 정도 (높을수록 부드러움 = 느림)

    [Header("Look At Settings")]
    [Tooltip("카메라가 바라볼 목표 지점의 오프셋 (플레이어의 발 기준).")]
    public Vector3 lookAtOffset = new Vector3(0f, 1f, 0f); // 카메라가 플레이어를 바라볼 기준점의 오프셋 (예: 플레이어의 명치 높이)

    void Start()
    {
        // target이 할당되지 않았다면 "Player" 태그를 가진 오브젝트를 찾습니다.
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogError("SmoothFollowCamera: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다. target에 플레이어를 할당해주세요.");
                this.enabled = false; // 스크립트 비활성화
                return;
            }
        }
    }

    // FixedUpdate는 물리 계산이나 부드러운 카메라 이동에 적합합니다.
    // LateUpdate는 모든 Update 함수가 실행된 후 호출되므로, 플레이어 이동 후 카메라가 따라가게 할 때 적합합니다.
    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산: 플레이어 위치 + 오프셋
        Vector3 desiredPosition = target.position + offset;

        // 현재 카메라 위치에서 목표 위치로 부드럽게 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 카메라가 바라볼 목표 지점 계산: 플레이어 위치 + 바라볼 오프셋
        Vector3 lookAtTarget = target.position + lookAtOffset;

        // 카메라가 목표 지점을 바라보도록 설정
        transform.LookAt(lookAtTarget);
    }

    // 유니티 에디터에서 기즈모로 카메라가 바라볼 위치를 시각화
    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            // 카메라가 바라볼 플레이어의 목표 지점
            Vector3 lookAtTargetPosition = target.position + lookAtOffset;
            Gizmos.DrawSphere(lookAtTargetPosition, 0.2f); // 작은 구로 표시

            Gizmos.color = Color.yellow;
            // 카메라의 현재 위치와 목표 위치를 연결하는 선
            Gizmos.DrawLine(transform.position, lookAtTargetPosition);

            Gizmos.color = Color.green;
            // 플레이어로부터 카메라가 있을 desiredPosition
            Vector3 desiredPosFromPlayer = target.position + offset;
            Gizmos.DrawSphere(desiredPosFromPlayer, 0.1f); // 작은 구로 표시
            Gizmos.DrawLine(target.position, desiredPosFromPlayer); // 플레이어와 카메라 목표 위치 연결
        }
    }
}