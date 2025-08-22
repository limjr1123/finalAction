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
            // DontDestroyOnLoad에서 현재 씬으로 이동하지 않고 그대로 유지
            Debug.Log("플레이어는 DontDestroyOnLoad 영역에 유지됩니다.");

            // 만약 위치만 조정하고 싶다면:
            // player.transform.position = 원하는위치;
        }
    }

    public void TransformPlayerCoroutine(GameObject _player)
    {
        player = _player;
        StartCoroutine(TransformPlayer(player));
    }
}
