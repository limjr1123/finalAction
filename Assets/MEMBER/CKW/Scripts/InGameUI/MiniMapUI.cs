using TMPro;        // TextMeshPro UI 컴포넌트 사용을 위한 네임스페이스
using UnityEngine;  // Unity 엔진 기본 기능 사용을 위한 네임스페이스
using UnityEngine.UI; // Unity UI 시스템 사용을 위한 네임스페이스

public class MinimapUI : MonoBehaviour
{
    [Header("UI References")] // 인스펙터에서 UI 참조 섹션 표시
    public RawImage minimapDisplay;    // 미니맵을 표시할 RawImage UI 컴포넌트
    public TextMeshProUGUI mapNameText; // 맵 이름을 표시할 TextMeshPro UI 텍스트
    public Camera minimapCamera;       // 미니맵을 렌더링할 카메라

    [Header("Map Settings")] // 인스펙터에서 맵 설정 섹션 표시
    public string currentMapName = "빅토리아로드"; // 현재 맵의 이름

    private RenderTexture renderTexture; // 미니맵 카메라가 렌더링할 텍스처

    void Start()
    {
        SetupMinimap(); // 게임 시작 시 미니맵 초기화
    }

    void SetupMinimap()
    {
        // 128x128 해상도, 16비트 깊이 버퍼를 가진 렌더 텍스처 생성
        renderTexture = new RenderTexture(128, 128, 16);

        // 렌더 텍스처를 GPU 메모리에 실제로 생성
        renderTexture.Create();

        // 미니맵 카메라가 이 렌더 텍스처에 장면을 렌더링하도록 설정
        minimapCamera.targetTexture = renderTexture;

        // UI의 RawImage에 렌더 텍스처를 할당하여 미니맵 표시
        minimapDisplay.texture = renderTexture;
    }

    public void UpdateMapName()
    {
        // 맵 이름 텍스트 UI가 존재하는지 확인
        if (mapNameText != null)
            mapNameText.text = currentMapName; // 현재 맵 이름을 UI 텍스트에 표시
    }

    public void ChangeMap(string newMapName)
    {
        currentMapName = newMapName; // 새로운 맵 이름으로 변경
        UpdateMapName();             // UI에 변경된 맵 이름 업데이트
    }
}