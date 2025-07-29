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
        // 비동기 로딩
        Instance.StartCoroutine(Instance.LoadSceneAsyncRoutine(sceneName));
    }

    private System.Collections.IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            // op.progress : 0.0 ~ 0.9 (씬 로딩), 0.9~1.0 (전환 준비)
            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Field")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // 등록 해제 (중복 방지)

            // 씬 로드 완료 후 실행할 함수
            GameManager.Instance.LoadGame();
            Debug.Log("Field 씬 로드 후 함수 호출 완료!");
        }
    }
}
