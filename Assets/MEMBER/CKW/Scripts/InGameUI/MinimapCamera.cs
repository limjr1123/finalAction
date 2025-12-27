using UnityEngine; // Unity 엔진 기본 기능 사용을 위한 네임스페이스

public class MinimapCamera : MonoBehaviour
{
    public Transform player; // 추적할 플레이어의 Transform 컴포넌트
    public float height = 20f; // 플레이어 위 높이 (미니맵 카메라가 위치할 높이)

    void LateUpdate()
    {
        if (player == null)
        {
            GameObject pl = GameObject.FindWithTag("Player");
            player = pl.transform;
        }
        // 플레이어 Transform이 할당되어 있는지 확인
        if (player != null)
        {
            // 플레이어의 현재 위치를 가져옴
            Vector3 newPosition = player.position;

            // Y축 좌표만 플레이어 위치 + 설정된 높이로 변경 (플레이어 위에 위치)
            newPosition.y = player.position.y + height;

            // 미니맵 카메라를 새로운 위치로 이동
            transform.position = newPosition;

            // 카메라가 아래쪽을 바라보도록 회전 설정 (X축으로 90도 회전)
            // Euler(90, 0, 0) = X축 90도 회전으로 아래를 내려다보는 각도
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}