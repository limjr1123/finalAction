using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환 시 필요할 수 있음

public class SavePoint : MonoBehaviour
{
    [Header("Save Point Settings")]
    public string savePointID = "DefaultSavePoint"; // 각 세이브 포인트를 식별할 고유 ID
    public GameObject player; // 저장할 플레이어 오브젝트 (Inspector에서 할당)
    public float saveDelay = 0.5f; // 플레이어가 세이브 포인트에 들어간 후 저장까지의 딜레이

    private bool playerInRange = false;

    void Start()
    {
        // 플레이어 오브젝트가 할당되지 않았다면, "Player" 태그를 가진 오브젝트를 찾음
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("SavePoint: Player GameObject not found. Please assign it in the Inspector or tag your player with 'Player'.");
            }
        }

        // 세이브 포인트 오브젝트에 Collider가 없으면 경고
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("SavePoint: This SavePoint needs a Collider component with 'Is Trigger' enabled to detect player entry.", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 세이브 포인트 범위에 들어오면 저장
        if (other.gameObject == player)
        {
            if (!playerInRange) // 중복 저장을 방지
            {
                playerInRange = true;
                Debug.Log($"플레이어 '{player.name}'가 세이브 포인트 '{savePointID}'에 진입했습니다. 잠시 후 저장됩니다.");
                Invoke("PerformSave", saveDelay); // 딜레이 후 저장 함수 호출
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 플레이어가 세이브 포인트 범위에서 벗어나면 플래그 리셋
        if (other.gameObject == player)
        {
            playerInRange = false;
            Debug.Log($"플레이어 '{player.name}'가 세이브 포인트 '{savePointID}'에서 벗어났습니다.");
            CancelInvoke("PerformSave"); // 저장 딜레이 중 나갔다면 취소
        }
    }

    void PerformSave()
    {
        if (player == null) return;

        // 1. 플레이어 위치/로테이션 저장
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", player.transform.position.z);
        PlayerPrefs.SetFloat("PlayerRotY", player.transform.rotation.eulerAngles.y); // Y축 회전만 저장 (캐릭터 방향)

        // 2. 현재 씬 이름 저장 (씬 로드시 사용)
        PlayerPrefs.SetString("LastSceneName", SceneManager.GetActiveScene().name);

        // 3. (선택 사항) 게임 진행 상황 저장 예시
        // 이 부분은 게임의 다른 스크립트에서 PlayerPrefabs를 통해 접근하거나,
        // SaveManager와 같은 중앙 관리 스크립트를 통해 저장/로드하는 것이 일반적
        // 여기서는 예시로 'Coins'와 'QuestProgress'를 저장한다고 가정
        PlayerPrefs.SetInt("PlayerCoins", 100); // 현재 코인 수 (예시)
        PlayerPrefs.SetInt("QuestProgress", 2); // 퀘스트 진행 단계 (예시)

        // 4. PlayerPrefs 변경사항 적용 (매우 중요!)
        PlayerPrefs.Save();

        Debug.Log($"<color=lime>게임이 '{savePointID}' 지점에 저장되었습니다!</color>");

        // 저장 완료 메시지 표시 또는 사운드 재생 등 추가 효과
        // GetComponent<AudioSource>()?.Play();
    }

    // 외부에서 강제로 저장 로직을 호출해야 할 경우를 대비 (선택 사항)
    public void ForceSave()
    {
        PerformSave();
    }
}