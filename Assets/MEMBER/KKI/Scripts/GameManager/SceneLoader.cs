using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadSceneAsync(string sceneName)
    {
        Instance.StartCoroutine(Instance.LoadSceneWithLoadingUI(sceneName));
    }

    // 씬 로딩과 함께 로딩 UI를 보여주는 메인 함수
    private IEnumerator LoadSceneWithLoadingUI(string sceneName)
    {
        // 1. 먼저 로딩 UI를 즉시 활성화 (로딩 화면 띄우기)
        LoadingManager.Instance.StartLoading();

        // 2. 로딩 UI가 완전히 나타날 때까지 잠깐 대기 (페이드인 효과 보여주기)
        yield return new WaitForSeconds(0.2f);

        // 3. 가짜 로딩 시뮬레이션 (너무 빨리 끝나는 걸 방지해서 자연스럽게 만들기)
        yield return StartCoroutine(SimulateLoading());

        // 4. 실제 씬 로딩 시작 (비동기로 백그라운드에서 새로운 씬 불러오기)
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        // 5. 씬 로딩이 완료될 때까지 진행도 실시간 업데이트
        while (!op.isDone) // 씬 로딩이 완료되지 않았으면 계속 반복
        {
            // 실제 진행도를 80%~100%에 매핑 (앞의 80%는 가짜 시뮬레이션이었음)
            // op.progress는 0.0~0.9 범위이므로 이를 20% 구간(0.8~1.0)에 맞춤
            float realProgress = 0.8f + (op.progress / 0.9f) * 0.2f;
            LoadingManager.Instance.UpdateProgress(realProgress); // 진행률 업데이트
            yield return null; // 다음 프레임까지 대기 (매 프레임마다 체크)
        }

        // 6. 100% 완료 표시
        LoadingManager.Instance.UpdateProgress(1f); // 진행률을 완전히 100%로 설정
        yield return new WaitForSeconds(0.5f); // 100% 상태를 0.5초간 보여주기 (만족감 제공)

        // 7. 로딩 UI 비활성화 (페이드아웃 효과로 로딩 화면 사라지게 하기)
        LoadingManager.Instance.EndLoading();
    }

    // 가짜 로딩 시뮬레이션 함수 (너무 빠른 로딩을 방지해서 자연스럽게 만들기)
    private IEnumerator SimulateLoading()
    {
        float progress = 0f; // 현재 가짜 진행률 (0%부터 시작)
        float targetProgress = 0.8f; // 목표 진행률 (80%까지만 가짜로 채움)

        // 진행률이 80%에 도달할 때까지 반복
        while (progress < targetProgress)
        {
            // 점진적으로 증가 (빠른 시작, 느린 끝 - 자연스러운 로딩 느낌)
            // 2f - progress: 처음엔 빠르게(2), 나중엔 느리게(1.2, 0.8...) 증가
            progress += Time.deltaTime * (2f - progress); // 적응형 속도 (시간이 갈수록 느려짐)
            progress = Mathf.Min(progress, targetProgress); // 80%를 넘지 않도록 제한

            LoadingManager.Instance.UpdateProgress(progress); // 현재 진행률을 UI에 반영
            yield return null; // 다음 프레임까지 대기 (부드러운 애니메이션)
        }
    }

    // 씬이 로드되었을 때 자동으로 호출되는 콜백 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 로드된 씬이 "Field"(필드) 또는 "Dungeon"(던전)인지 확인
        if (scene.name == "Field" || scene.name == "Dungeon")
        {
            // 이벤트 중복 호출을 막기 위해 등록을 해제 (메모리 누수 방지)
            SceneManager.sceneLoaded -= OnSceneLoaded;

            // 게임 데이터를 로드 (플레이어 정보, 아이템, 진행상황 등 불러오기)
            GameManager.Instance.LoadGame();

            // 어떤 씬이 로드되었는지 디버그 로그로 확인 (개발용)
            Debug.Log($"{scene.name} 씬 로드 후 함수 호출 완료!");
        }
    }
}