// PlayerManager.cs
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // 이 스크립트의 유일한 인스턴스를 저장하기 위한 static 변수
    public static PlayerManager instance;

    void Awake()
    {
        // 만약 instance가 아직 할당되지 않았다면
        if (instance == null)
        {
            // 이 인스턴스를 static 변수에 할당
            instance = this;
            // 씬이 전환되어도 이 게임 오브젝트(플레이어)를 파괴하지 않음
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 만약 instance가 이미 존재한다면 (예: 씬을 다시 로드한 경우)
            // 중복 생성을 막기 위해 이 게임 오브젝트는 파괴함
            Destroy(gameObject);
        }
    }
}