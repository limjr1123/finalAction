using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraZoom : MonoBehaviour
{
    private CinemachineCamera vcam;

    [Header("줌 설정")]
    [SerializeField]
    private float zoomSpeed = 20f;   // 줌 속도
    [SerializeField]
    private float pinchZoomSpeed = 0.02f; // 핀치 줌 감도
    [SerializeField]
    private float minZoom = 30f;     // 최소 줌 (가장 가까이)
    [SerializeField]
    private float maxZoom = 60f;     // 최대 줌 (가장 멀리)

    private float targetZoom;       // 목표 줌 값

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();

        targetZoom = vcam.Lens.FieldOfView;
    }

    void Update()
    {
        //------------ PC 기준---------------------------------------------------

        // 1. 마우스 휠 입력 받기
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        // 2. 목표 줌 값 계산
        targetZoom -= scrollInput * zoomSpeed;

        //------------ 모바일 기준---------------------------------------------------

        //if (Input.touchCount == 2)
        //{
        //    // 두 개의 터치 정보를 가져옵니다.
        //    Touch touchZero = Input.GetTouch(0);
        //    Touch touchOne = Input.GetTouch(1);

        //    // 현재 프레임과 이전 프레임에서의 두 터치 지점 위치를 찾습니다.
        //    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        //    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        //    // 이전 프레임과 현재 프레임의 두 터치 사이의 거리를 계산합니다.
        //    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        //    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        //    // 거리의 차이를 계산하여 줌의 변화량으로 사용합니다.
        //    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        //    // 목표 줌 값을 변경합니다.
        //    targetZoom += deltaMagnitudeDiff * pinchZoomSpeed;
        //}

        //---------------------------------------------------------------------------------

        // 3. 줌 값 범위 제한
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // 4. 부드러운 줌 효과 적용
        // 시네머신 가상 카메라의 Lens 설정에 있는 Field of View 값을 변경
        vcam.Lens.FieldOfView = Mathf.Lerp(vcam.Lens.FieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }
}