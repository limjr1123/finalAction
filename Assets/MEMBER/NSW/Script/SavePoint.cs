using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요합니다.

/// <summary>
/// 플레이어가 진입 시, 지정된 RespawnPoint의 위치를 저장하는 스크립트.
/// </summary>
public class SavePoint : MonoBehaviour
{
    [Header("Save Point Settings")]
    [Tooltip("각 세이브 포인트를 식별할 고유 ID (선택 사항)")]
    public string savePointID = "DefaultSavePoint";

    [Tooltip("이 세이브 포인트와 짝을 이루는 리스폰 포인트 오브젝트의 Transform")]
    public Transform respawnPoint;

    [Tooltip("플레이어가 진입 후 저장까지 걸리는 시간(초)")]
    public float saveDelay = 0.5f;

    // 플레이어가 범위 내에 있는지 확인하여 중복 저장을 방지하는 플래그
    private bool playerInRange = false;

    /// <summary>
    /// 스크립트가 처음 활성화될 때 호출됩니다.
    /// 필요한 컴포넌트나 설정이 올바른지 확인합니다.
    /// </summary>
    void Start()
    {
        // 리스폰 포인트가 Inspector에서 할당되었는지 확인
        if (respawnPoint == null)
        {
            // respawnPoint 변수가 비어있으면 에러 메시지를 콘솔에 출력합니다.
            Debug.LogError($"SavePoint '{savePointID}'에 RespawnPoint가 할당되지 않았습니다! Inspector에서 설정해주세요.", this);
        }

        // 이 오브젝트에 Collider가 있고, Is Trigger가 켜져 있는지 확인
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            // 트리거 설정이 없으면 경고 메시지를 콘솔에 출력합니다.
            Debug.LogWarning("SavePoint: 이 오브젝트는 'Is Trigger'가 활성화된 Collider가 필요합니다.", this);
        }
    }

    /// <summary>
    /// 다른 Collider가 이 오브젝트의 트리거 범위 안으로 들어왔을 때 호출됩니다.
    /// </summary>
    /// <param name="other">트리거에 들어온 다른 오브젝트의 Collider</param>
    void OnTriggerEnter(Collider other)
    {
        // 들어온 오브젝트의 태그가 "Player"인지 확인하고, 아직 범위 안에 들어오지 않았다면
        if (other.CompareTag("Player") && !playerInRange)
        {
            playerInRange = true; // 플레이어가 범위에 들어왔다고 표시
            Debug.Log($"플레이어가 세이브 포인트 '{savePointID}'에 진입했습니다.");

            // 지정된 딜레이 이후에 PerformSave 함수를 호출
            Invoke("PerformSave", saveDelay);
        }
    }

    /// <summary>
    /// 다른 Collider가 이 오브젝트의 트리거 범위 밖으로 나갔을 때 호출됩니다.
    /// </summary>
    /// <param name="other">트리거에서 나간 다른 오브젝트의 Collider</param>
    void OnTriggerExit(Collider other)
    {
        // 나간 오브젝트의 태그가 "Player"라면
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // 플레이어가 범위를 벗어났다고 표시
        }
    }

    /// <summary>
    /// 실제 저장 로직을 수행하는 함수.
    /// </summary>
    void PerformSave()
    {
        // respawnPoint가 할당되지 않았다면 함수를 즉시 종료
        if (respawnPoint == null) return;

        // 1. 플레이어 위치 대신, 연결된 리스폰 포인트의 위치를 저장
        PlayerPrefs.SetFloat("PlayerPosX", respawnPoint.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", respawnPoint.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", respawnPoint.position.z);
        PlayerPrefs.SetFloat("PlayerRotY", respawnPoint.rotation.eulerAngles.y); // 리스폰 포인트의 Y축 회전값 저장

        // 2. 현재 씬 이름 저장
        PlayerPrefs.SetString("LastSceneName", SceneManager.GetActiveScene().name);

        // 3. PlayerPrefs의 모든 변경사항을 디스크에 실제로 저장 (매우 중요!)
        PlayerPrefs.Save();

        // 저장 완료 로그를 라임색(lime)으로 보기 좋게 출력
        Debug.Log($"<color=lime>게임 저장 완료! 리스폰 위치({respawnPoint.position})가 설정되었습니다.</color>");
    }
}