using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    protected override void Awake()
    {
        base.Awake();
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
}
