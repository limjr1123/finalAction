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
        // 1. 마우스 휠 입력 받기
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // 2. 목표 줌 값 계산
        targetZoom -= scrollInput * zoomSpeed;

        // 3. 줌 값 범위 제한
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // 4. 부드러운 줌 효과 적용
        // 시네머신 가상 카메라의 Lens 설정에 있는 Field of View 값을 변경
        vcam.Lens.FieldOfView = Mathf.Lerp(vcam.Lens.FieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }
}