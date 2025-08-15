using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferManager : Singleton<TransferManager>
{
    private GameObject player;

    public IEnumerator TransformPlayer(GameObject player)
    {
        yield return new WaitForSeconds(5f);

        if (player != null)
        {
            // 현재 씬의 루트로 이동 (null을 전달하면 현재 씬으로 이동)
            Scene activeScene = SceneManager.GetActiveScene();

            if (player != null && activeScene.isLoaded)
            {
                SceneManager.MoveGameObjectToScene(player, activeScene);
                Debug.Log("플레이어가 DontDestroyOnLoad에서 현재 씬으로 이동됨");
            }
        }
    }

    public void TransformPlayerCoroutine(GameObject _player)
    {
        player = _player;
        StartCoroutine(TransformPlayer(player));
    }
}
