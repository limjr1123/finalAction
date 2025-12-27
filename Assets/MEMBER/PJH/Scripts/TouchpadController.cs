using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Cinemachine;

public class TouchpadController : MonoBehaviour, IDragHandler, IPointerUpHandler
{
    [Header("XY 축 컨트롤러")]
    public CinemachineInputAxisController xAxisController;
    public CinemachineInputAxisController yAxisController;

    [Header("회전 감도")]
    public float sensitivity = 0.02f;

    void Start()
    {
        // 씬에 있는 활성화된 CinemachineCamera를 찾습니다.  
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();

        if (cam != null)
        {
            // CinemachineCamera에서 X축과 Y축의 InputAxisController를 가져와 할당합니다.  
            xAxisController = cam.GetComponent<CinemachineInputAxisController>();
            yAxisController = cam.GetComponent<CinemachineInputAxisController>();
            Debug.Log("터치패드: 시네머신 축 컨트롤러를 자동으로 연결했습니다.");
        }
        else
        {
            Debug.LogError("터치패드: 씬에서 CinemachineCamera를 찾을 수 없습니다!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (xAxisController == null || yAxisController == null) return;

        // 각 축 컨트롤러의 Value 속성에 직접 값을 넣어줍니다.  
        xAxisController.Controllers[0].InputValue = eventData.delta.x * sensitivity;
        yAxisController.Controllers[0].InputValue = eventData.delta.y * sensitivity;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (xAxisController == null || yAxisController == null) return;

        // 입력 값을 0으로 초기화하여 카메라가 계속 회전하는 것을 방지합니다.  
        xAxisController.Controllers[0].InputValue = 0;
        yAxisController.Controllers[0].InputValue = 0;
    }
}
