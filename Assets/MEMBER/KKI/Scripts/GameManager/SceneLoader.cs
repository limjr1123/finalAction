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

    private IEnumerator LoadSceneWithLoadingUI(string sceneName)
    {
        // 1. 먼저 로딩 UI를 즉시 활성화
        LoadingManager.Instance.StartLoading();

        // 2. 로딩 UI가 완전히 나타날 때까지 대기
        yield return new WaitForSeconds(0.2f);

        // 3. 가짜 로딩 시뮬레이션 (너무 빠른 로딩 방지)
        yield return StartCoroutine(SimulateLoading());

        // 4. 실제 씬 로딩 시작
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        // 5. 씬 로딩 진행도 업데이트
        while (!op.isDone)
        {
            // 실제 진행도를 80%~100%에 매핑 (앞의 80%는 시뮬레이션)
            float realProgress = 0.8f + (op.progress / 0.9f) * 0.2f;
            LoadingManager.Instance.UpdateProgress(realProgress);
            yield return null;
        }

        // 6. 100% 완료
        LoadingManager.Instance.UpdateProgress(1f);
        yield return new WaitForSeconds(0.5f); // 100% 잠깐 보여주기

        // 7. 로딩 UI 비활성화
        LoadingManager.Instance.EndLoading();
    }

    // 가짜 로딩 시뮬레이션 (너무 빠른 로딩을 방지)
    private IEnumerator SimulateLoading()
    {
        float progress = 0f;
        float targetProgress = 0.8f; // 80%까지 시뮬레이션

        while (progress < targetProgress)
        {
            // 점진적으로 증가 (빠른 시작, 느린 끝)
            progress += Time.deltaTime * (2f - progress); // 적응형 속도
            progress = Mathf.Min(progress, targetProgress);

            LoadingManager.Instance.UpdateProgress(progress);
            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 로드된 씬이 "Field" 또는 "Dungeon"인지 확인합니다.
        if (scene.name == "Field" || scene.name == "Dungeon")
        {
            // 이벤트 중복 호출을 막기 위해 등록을 해제합니다.
            SceneManager.sceneLoaded -= OnSceneLoaded;
            // 게임 데이터를 로드합니다.
            GameManager.Instance.LoadGame();
            // 어떤 씬이 로드되었는지 정확히 로그로 남깁니다.
            Debug.Log($"{scene.name} 씬 로드 후 함수 호출 완료!");
        }
    }
}