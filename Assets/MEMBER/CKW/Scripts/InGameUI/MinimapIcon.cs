using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    // 아이콘 타입을 정의하는 열거형
    public enum IconType
    {
        Player,  // 플레이어
        NPC,     // NPC
        Enemy    // 적
    }

    [Header("Icon Settings")]
    public IconType iconType = IconType.Player;  // 아이콘 타입 설정
    public float iconSize = 0.5f;                // 아이콘 크기 설정

    private GameObject iconObject;     // 생성될 아이콘 오브젝트
    private MeshRenderer iconRenderer; // 아이콘의 메시 렌더러

    void Start()
    {
        CreateIcon(); // 게임 시작 시 아이콘 생성
    }

    void CreateIcon()
    {
        // 구 형태의 기본 프리미티브로 아이콘용 오브젝트 생성
        iconObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // 아이콘 오브젝트의 이름을 부모 오브젝트 이름 + "_MinimapIcon"으로 설정
        iconObject.name = gameObject.name + "_MinimapIcon";

        // 아이콘을 현재 오브젝트의 자식으로 설정
        iconObject.transform.SetParent(transform);

        // 아이콘의 로컬 위치를 부모 중심(0,0,0)으로 설정
        iconObject.transform.localPosition = Vector3.zero;

        // 아이콘의 크기를 설정된 iconSize 값으로 조정
        iconObject.transform.localScale = Vector3.one * iconSize;

        // 아이콘을 "MinimapIcons" 레이어로 설정 (미니맵에서만 보이도록)
        iconObject.layer = LayerMask.NameToLayer("MinimapIcons");

        // 물리적 충돌을 방지하기 위해 콜리더 컴포넌트 제거
        Destroy(iconObject.GetComponent<Collider>());

        // 아이콘의 메시 렌더러 컴포넌트 가져오기
        iconRenderer = iconObject.GetComponent<MeshRenderer>();

        // 아이콘 색상 설정 함수 호출
        SetIconColor();
    }

    void SetIconColor()
    {
        // 단색으로 표시하기 위한 Unlit/Color 셰이더로 새 머티리얼 생성
        Material iconMaterial = new Material(Shader.Find("Unlit/Color"));

        // 기본 아이콘 색상을 흰색으로 설정
        Color iconColor = Color.white;

        // 아이콘 타입에 따라 색상 결정
        switch (iconType)
        {
            case IconType.Player: // 플레이어인 경우
                iconColor = Color.yellow; // 노란색으로 설정
                break;
            case IconType.NPC:    // NPC인 경우
                iconColor = Color.green;  // 초록색으로 설정
                break;
            case IconType.Enemy:  // 적인 경우
                iconColor = Color.red;    // 빨간색으로 설정
                break;
        }

        // 머티리얼에 결정된 색상 적용
        iconMaterial.color = iconColor;

        // 아이콘 렌더러에 머티리얼 적용
        iconRenderer.material = iconMaterial;

        // 아이콘이 그림자를 생성하지 않도록 설정
        iconRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        // 아이콘이 다른 오브젝트의 그림자를 받지 않도록 설정
        iconRenderer.receiveShadows = false;
    }

    void LateUpdate()
    {
        // 매 프레임 마지막에 실행되어 아이콘이 항상 월드 좌표 기준으로 위를 향하도록 회전 고정
        if (iconObject != null) // 아이콘 오브젝트가 존재하는지 확인
        {
            iconObject.transform.rotation = Quaternion.identity; // 회전값을 기본값(0,0,0)으로 설정
        }
    }
}