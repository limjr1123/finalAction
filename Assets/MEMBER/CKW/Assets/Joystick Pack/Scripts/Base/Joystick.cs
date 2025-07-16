using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // 조이스틱의 수평 입력값 반환 (snapX가 true면 스냅 처리된 값)
    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }

    // 조이스틱의 수직 입력값 반환 (snapY가 true면 스냅 처리된 값)
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }

    // 조이스틱의 방향 벡터 반환 (Horizontal, Vertical을 Vector2로 결합)
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    // 조이스틱 핸들의 움직임 범위 설정/반환 (절댓값으로 처리)
    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }

    // 데드존 범위 설정/반환 (절댓값으로 처리)
    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    // 축 옵션 설정/반환 (Both, Horizontal, Vertical 중 선택)
    public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }

    // X축 스냅 여부 설정/반환
    public bool SnapX { get { return snapX; } set { snapX = value; } }

    // Y축 스냅 여부 설정/반환
    public bool SnapY { get { return snapY; } set { snapY = value; } }

    // 조이스틱 핸들의 움직임 범위 (0~1 사이 값)
    [SerializeField] private float handleRange = 1;

    // 입력이 무시되는 데드존 범위
    [SerializeField] private float deadZone = 0;

    // 입력 축 옵션 (Both: 양방향, Horizontal: 수평만, Vertical: 수직만)
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;

    // X축 스냅 활성화 여부 (true시 -1, 0, 1로 스냅)
    [SerializeField] private bool snapX = false;

    // Y축 스냅 활성화 여부 (true시 -1, 0, 1로 스냅)
    [SerializeField] private bool snapY = false;

    // 조이스틱 배경 RectTransform
    [SerializeField] protected RectTransform background = null;

    // 조이스틱 핸들 RectTransform
    [SerializeField] private RectTransform handle = null;

    // 조이스틱 베이스 RectTransform
    private RectTransform baseRect = null;

    // 조이스틱이 속한 캔버스 참조
    private Canvas canvas;

    // 렌더링에 사용되는 카메라
    private Camera cam;

    // 현재 입력값 저장
    private Vector2 input = Vector2.zero;

    // 초기화 메서드
    protected virtual void Start()
    {
        // 핸들 범위와 데드존 설정
        HandleRange = handleRange;
        DeadZone = deadZone;

        // 베이스 RectTransform 컴포넌트 가져오기
        baseRect = GetComponent<RectTransform>();

        // 부모 캔버스 찾기
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        // 중심점 설정 (0.5, 0.5)
        Vector2 center = new Vector2(0.5f, 0.5f);

        // 배경의 피벗을 중심으로 설정
        background.pivot = center;

        // 핸들의 앵커를 중심으로 설정
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;

        // 핸들 초기 위치를 중심으로 설정
        handle.anchoredPosition = Vector2.zero;
    }

    // 포인터(마우스/터치) 다운 이벤트 처리
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // 드래그 이벤트 호출
        OnDrag(eventData);
    }

    // 드래그 이벤트 처리 (조이스틱 입력의 핵심 로직)
    public void OnDrag(PointerEventData eventData)
    {
        cam = null;

        // 캔버스 렌더 모드가 ScreenSpaceCamera인 경우 카메라 설정
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        // 배경의 월드 좌표를 스크린 좌표로 변환
        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);

        // 배경의 반지름 계산
        Vector2 radius = background.sizeDelta / 2;

        // 입력값 계산 (터치 위치 - 배경 중심) / (반지름 * 캔버스 스케일)
        input = (eventData.position - position) / (radius * canvas.scaleFactor);

        // 입력값 포맷팅 (축 옵션에 따른 제한)
        FormatInput();

        // 입력 처리 (상속 클래스에서 오버라이드 가능)
        HandleInput(input.magnitude, input.normalized, radius, cam);

        // 핸들 위치 업데이트
        handle.anchoredPosition = input * radius * handleRange;
    }

    // 입력 처리 메서드 (상속 클래스에서 오버라이드 가능)
    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        // 데드존보다 큰 입력만 처리
        if (magnitude > deadZone)
        {
            // 입력값이 1보다 크면 정규화
            if (magnitude > 1)
                input = normalised;
        }
        else
            // 데드존 내부면 입력값 0으로 설정
            input = Vector2.zero;
    }

    // 축 옵션에 따른 입력값 포맷팅
    private void FormatInput()
    {
        // 수평 축만 사용하는 경우
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        // 수직 축만 사용하는 경우
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    // 스냅 처리 메서드 (입력값을 -1, 0, 1로 스냅)
    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        // 값이 0이면 그대로 반환
        if (value == 0)
            return value;

        // 양방향 축 사용 시
        if (axisOptions == AxisOptions.Both)
        {
            // 입력 벡터와 위쪽 방향의 각도 계산
            float angle = Vector2.Angle(input, Vector2.up);

            // 수평 축 스냅 처리
            if (snapAxis == AxisOptions.Horizontal)
            {
                // 각도가 22.5도 미만이거나 157.5도 초과면 0 반환 (거의 수직)
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    // 아니면 -1 또는 1 반환
                    return (value > 0) ? 1 : -1;
            }
            // 수직 축 스냅 처리
            else if (snapAxis == AxisOptions.Vertical)
            {
                // 각도가 67.5도~112.5도 사이면 0 반환 (거의 수평)
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    // 아니면 -1 또는 1 반환
                    return (value > 0) ? 1 : -1;
            }
            // 그 외의 경우 원래값 반환
            return value;
        }
        // 단일 축 사용 시
        else
        {
            // 양수면 1, 음수면 -1 반환
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }

    // 포인터 업 이벤트 처리 (터치/마우스 버튼 뗄 때)
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        // 입력값 초기화
        input = Vector2.zero;

        // 핸들을 중심으로 되돌리기
        handle.anchoredPosition = Vector2.zero;
    }

    // 스크린 좌표를 앵커 포지션으로 변환하는 헬퍼 메서드
    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;

        // 스크린 좌표를 로컬 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
        {
            // 피벗 오프셋 계산
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;

            // 최종 앵커 포지션 반환
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }
}

// 축 옵션 열거형
public enum AxisOptions
{
    Both,       // 양방향 (X, Y 모두)
    Horizontal, // 수평 (X축만)
    Vertical    // 수직 (Y축만)
}