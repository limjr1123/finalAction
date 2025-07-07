using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ �� �ʿ��� �� ����

public class SavePoint : MonoBehaviour
{
    [Header("Save Point Settings")]
    public string savePointID = "DefaultSavePoint"; // �� ���̺� ����Ʈ�� �ĺ��� ���� ID
    public GameObject player; // ������ �÷��̾� ������Ʈ (Inspector���� �Ҵ�)
    public float saveDelay = 0.5f; // �÷��̾ ���̺� ����Ʈ�� �� �� ��������� ������

    private bool playerInRange = false;

    void Start()
    {
        // �÷��̾� ������Ʈ�� �Ҵ���� �ʾҴٸ�, "Player" �±׸� ���� ������Ʈ�� ã��
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("SavePoint: Player GameObject not found. Please assign it in the Inspector or tag your player with 'Player'.");
            }
        }

        // ���̺� ����Ʈ ������Ʈ�� Collider�� ������ ���
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("SavePoint: This SavePoint needs a Collider component with 'Is Trigger' enabled to detect player entry.", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ���̺� ����Ʈ ������ ������ ����
        if (other.gameObject == player)
        {
            if (!playerInRange) // �ߺ� ������ ����
            {
                playerInRange = true;
                Debug.Log($"�÷��̾� '{player.name}'�� ���̺� ����Ʈ '{savePointID}'�� �����߽��ϴ�. ��� �� ����˴ϴ�.");
                Invoke("PerformSave", saveDelay); // ������ �� ���� �Լ� ȣ��
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // �÷��̾ ���̺� ����Ʈ �������� ����� �÷��� ����
        if (other.gameObject == player)
        {
            playerInRange = false;
            Debug.Log($"�÷��̾� '{player.name}'�� ���̺� ����Ʈ '{savePointID}'���� ������ϴ�.");
            CancelInvoke("PerformSave"); // ���� ������ �� �����ٸ� ���
        }
    }

    void PerformSave()
    {
        if (player == null) return;

        // 1. �÷��̾� ��ġ/�����̼� ����
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", player.transform.position.z);
        PlayerPrefs.SetFloat("PlayerRotY", player.transform.rotation.eulerAngles.y); // Y�� ȸ���� ���� (ĳ���� ����)

        // 2. ���� �� �̸� ���� (�� �ε�� ���)
        PlayerPrefs.SetString("LastSceneName", SceneManager.GetActiveScene().name);

        // 3. (���� ����) ���� ���� ��Ȳ ���� ����
        // �� �κ��� ������ �ٸ� ��ũ��Ʈ���� PlayerPrefabs�� ���� �����ϰų�,
        // SaveManager�� ���� �߾� ���� ��ũ��Ʈ�� ���� ����/�ε��ϴ� ���� �Ϲ���
        // ���⼭�� ���÷� 'Coins'�� 'QuestProgress'�� �����Ѵٰ� ����
        PlayerPrefs.SetInt("PlayerCoins", 100); // ���� ���� �� (����)
        PlayerPrefs.SetInt("QuestProgress", 2); // ����Ʈ ���� �ܰ� (����)

        // 4. PlayerPrefs ������� ���� (�ſ� �߿�!)
        PlayerPrefs.Save();

        Debug.Log($"<color=lime>������ '{savePointID}' ������ ����Ǿ����ϴ�!</color>");

        // ���� �Ϸ� �޽��� ǥ�� �Ǵ� ���� ��� �� �߰� ȿ��
        // GetComponent<AudioSource>()?.Play();
    }

    // �ܺο��� ������ ���� ������ ȣ���ؾ� �� ��츦 ��� (���� ����)
    public void ForceSave()
    {
        PerformSave();
    }
}