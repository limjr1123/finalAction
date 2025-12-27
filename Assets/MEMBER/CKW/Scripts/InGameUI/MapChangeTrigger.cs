using UnityEngine; // Unity 엔진 기본 기능 사용을 위한 네임스페이스
using UnityEngine.SceneManagement; // 씬 관리 기능 사용을 위한 네임스페이스

public class MapChangeTrigger : MonoBehaviour
{
    // 씬 이름과 맵 이름을 매핑하는 열거형
    public enum SceneMapPair
    {
        Field,    // 빅토리아로드
        Dungeon, // 헤네시스사냥터
        SleepyWood,      // 슬리피우드
        ElliniaTown,     // 엘리니아마을
        PerionTown       // 페리온마을
    }

    [Header("Map Settings")] // 인스펙터에서 맵 설정 섹션 표시
    public MinimapUI minimapUI; // 미니맵 UI를 제어할 MinimapUI 컴포넌트 참조

    void Start()
    {
        // 게임 시작 시 현재 씬에 맞는 맵 이름으로 설정
        UpdateMapNameFromScene();
    }

    void UpdateMapNameFromScene()
    {
        // 현재 활성화된 씬의 이름을 가져옴
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 씬 이름을 한국어 맵 이름으로 변환
        string mapName = GetMapNameFromSceneName(currentSceneName);

        // 미니맵 UI가 할당되어 있는지 확인
        if (minimapUI != null)
        {
            // 변환된 맵 이름으로 미니맵 업데이트
            minimapUI.ChangeMap(mapName);
        }
    }

    string GetMapNameFromSceneName(string sceneName)
    {

        // 씬 이름에 따라 해당하는 한국어 맵 이름 반환
        switch (sceneName)
        {
            case "Field":           // VictoriaRoad 씬
                return "봉구스 마을";

            case "Dungeon":   // HenesysHuntingGround 씬
                return "고대 무덤";

            case "sleepywood":             // SleepyWood 씬
                return "슬리피우드";

            case "elliniatown":            // ElliniaTown 씬
                return "엘리니아마을";

            case "periontown":             // PerionTown 씬
                return "페리온마을";

            default:                       // 등록되지 않은 씬인 경우
                return "알 수 없는 지역";    // 기본값 반환
        }
    }

    // 씬이 변경될 때 호출되는 이벤트 함수 (선택적 사용)
    void OnEnable()
    {
        // 씬 변경 이벤트에 함수 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 오브젝트가 비활성화될 때 이벤트 등록 해제 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 새로운 씬이 로드될 때마다 자동으로 맵 이름 업데이트
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 완료 후 맵 이름 업데이트
        UpdateMapNameFromScene();
    }

    // 외부에서 수동으로 맵 이름을 변경하고 싶을 때 사용하는 함수
    public void ChangeMap(string mapName)
    {
        // 미니맵 UI가 할당되어 있는지 확인
        if (minimapUI != null)
        {
            // 지정된 맵 이름으로 직접 변경
            minimapUI.ChangeMap(mapName);
        }
    }
}