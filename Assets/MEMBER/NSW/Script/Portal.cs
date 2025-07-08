using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // �ڷ�ƾ ����� ���� �ʿ�

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string portalID = "DefaultPortal"; // �� ��Ż�� ���� ID (�� ������ �ٸ� ��Ż�� �� ��Ż�� �������� ���� �� ���)

    [Header("Scene Transition")]
    [Tooltip("�� ��Ż�� ���� �̵��� ���� �̸� (�ʼ�). Build Settings�� �߰��Ǿ� �־�� �մϴ�.")]
    public string targetSceneName; // �̵��� �� �̸� (�ʼ�)
    [Tooltip("�� ��Ż�� ���� �̵����� ��, ������ ���� �ִ� ������ ��Ż�� ID�Դϴ�.")]
    public string targetPortalID; // ������ ���� �ִ� ������ ��Ż�� ���� ID

    [Header("Player Settings")]
    public GameObject player; // �÷��̾� ������Ʈ (Inspector���� �Ҵ� �Ǵ� "Player" �±׷� ã��)
    public float teleportDelay = 0.5f; // �÷��̾ ��Ż�� �� �� �̵������� ������
    public float fadeDuration = 0.5f; // ȭ�� ���̵� ��/�ƿ� �ð� (���� ������ ���� ��ũ��Ʈ ����)

    private bool playerInRange = false;
    private bool isTeleporting = false; // �ߺ� �ڷ���Ʈ ���� �÷���

    void Start()
    {
        // �÷��̾� ������Ʈ�� �Ҵ���� �ʾҴٸ�, "Player" �±׸� ���� ������Ʈ�� ã��
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Portal: Player GameObject not found. Please assign it in the Inspector or tag your player with 'Player'.");
            }
        }

        // ��Ż ������Ʈ�� Collider�� ������ ���
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("Portal: This Portal needs a Collider component with 'Is Trigger' enabled to detect player entry.", this);
        }

        // ������ �� �̸��� �������� �ʾ����� ��� (�� �̵� �����̹Ƿ� �ʼ�)
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError($"Portal '{portalID}': Target Scene Name is not set. This portal cannot function without a target scene!", this);
        }
        // ������ ��Ż ID�� �������� �ʾ����� ��� (�� �ε� �� �÷��̾ ��ġ�� �� �ʿ�)
        if (string.IsNullOrEmpty(targetPortalID))
        {
            Debug.LogWarning($"Portal '{portalID}': Target Portal ID is not set. Player might not spawn at the correct location in the new scene.", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && !isTeleporting)
        {
            if (!playerInRange) // �ߺ� Ʈ���� ����
            {
                playerInRange = true;
                isTeleporting = true; // �ڷ���Ʈ ���� �÷��� ����
                Debug.Log($"�÷��̾� '{player.name}'�� ��Ż '{portalID}'�� �����߽��ϴ�. �� '{targetSceneName}'�� '{targetPortalID}'�� �̵� �õ�.");
                StartCoroutine(TeleportPlayer());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
            // �÷��̾ ������ �� �ڷ���Ʈ�� ���� ������ �ʴٸ� �÷��� ����
            if (!isTeleporting)
            {
                Debug.Log($"�÷��̾� '{player.name}'�� ��Ż '{portalID}'���� ������ϴ�.");
            }
        }
    }

    IEnumerator TeleportPlayer()
    {
        // 1. �ڷ���Ʈ ������
        yield return new WaitForSeconds(teleportDelay);

        // 2. ȭ�� ���̵� �ƿ� (�ɼ�)
        // ���� ���ӿ����� ScreenFader ���� ���� ��ũ��Ʈ�� ����մϴ�.
        // ���� ��� ScreenFader.Instance.FadeOut(fadeDuration);
        Debug.Log("ȭ�� ���̵� �ƿ� ��...");
        yield return new WaitForSeconds(fadeDuration); // ���̵� �ƿ��� �Ϸ�� ������ ��ٸ� (�ӽ�)

        // �÷��̾� ������ ��Ȱ��ȭ (���� �̵� �� �̵� ����)
        SetPlayerMovement(false);

        // 3. ������ ������ PlayerPrefs�� ����
        // �� �ε� �� �÷��̾ �ùٸ� ��ġ�� ��ġ�ϱ� ���� �ʿ�
        PlayerPrefs.SetString("TargetPortalIDAfterSceneLoad", targetPortalID);
        PlayerPrefs.Save();

        // 4. �� �ε�
        // �� �ε尡 �Ϸ�Ǹ� OnSceneLoadedAndTeleport �Լ� ȣ���ϵ��� �̺�Ʈ ����
        SceneManager.sceneLoaded += OnSceneLoadedAndTeleport;
        SceneManager.LoadScene(targetSceneName);

        // isTeleporting �÷��״� �̵� �Ϸ� �� OnSceneLoadedAndTeleport �Լ����� ����
    }

    // �ٸ� �� �ε� �Ϸ� �� ȣ��� �Լ�
    void OnSceneLoadedAndTeleport(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"�� '{scene.name}' �ε� �Ϸ�. �ڷ���Ʈ ������ ã��.");
        // �̺�Ʈ �ڵ鷯 ���� (�ߺ� ȣ�� ����)
        SceneManager.sceneLoaded -= OnSceneLoadedAndTeleport;

        // PlayerPrefs���� ������ ��Ż ID �ٽ� ��������
        string storedTargetPortalID = PlayerPrefs.GetString("TargetPortalIDAfterSceneLoad", "");
        if (string.IsNullOrEmpty(storedTargetPortalID))
        {
            Debug.LogError("�� �ε� �� TargetPortalIDAfterSceneLoad �����Ͱ� PlayerPrefs�� �����ϴ�! �÷��̾ ��ġ�� �� �����ϴ�.");
            FinishTeleportation();
            return;
        }

        // ���� �ε�� ������ �÷��̾� ���� �ٽ� ��������
        // �÷��̾� ������Ʈ�� DontDestroyOnLoad ���� �ʾҴٸ�, ���ο� ������ �ٽ� ã�ƾ� ��.
        // �Ǵ� �÷��̾� �������� �ν��Ͻ�ȭ �ؾ� �� (PlayerRespawnManager�� ������ ����)
        if (player == null || !player.activeInHierarchy) // ���� ���� �÷��̾ ���ų� ��Ȱ��ȭ�Ǿ� �ִٸ�
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                // �� �κ��� PlayerRespawnManager�� �÷��̾� �������� �ν��Ͻ�ȭ�ϴ� ������ ����ϴ� ���� �� ����.
                // ���⼭�� �÷��̾ ���� �����Ѵٰ� �����ϰų�, GameManager�� �÷��̾ �����Ѵٰ� ����.
                Debug.LogError("���ο� ������ 'Player' �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�! �÷��̾� ��ġ ����.");
                FinishTeleportation();
                return;
            }
        }
        // �÷��̾� ������Ʈ�� ��Ȱ��ȭ�Ǿ� �־��ٸ� Ȱ��ȭ
        player.SetActive(true);


        // �ε�� ������ ������ ��Ż ã��
        Portal[] allPortalsInNewScene = FindObjectsOfType<Portal>();
        Portal targetPortal = null;

        foreach (Portal p in allPortalsInNewScene)
        {
            if (p.portalID == storedTargetPortalID)
            {
                targetPortal = p;
                break;
            }
        }

        if (targetPortal != null)
        {
            // �÷��̾� ��ġ �� ȸ�� ���� (��Ż�� �ٷ� ��)
            Vector3 destination = targetPortal.transform.position + targetPortal.transform.forward * 1.5f; // ��Ż �� 1.5 ����
            player.transform.position = destination;
            player.transform.rotation = targetPortal.transform.rotation;
            Debug.Log($"�÷��̾ ���ο� ���� '{targetPortal.portalID}' ��Ż ��ġ�� �̵����׽��ϴ�: {destination}");
        }
        else
        {
            Debug.LogError($"���ο� �� '{scene.name}'���� ������ ��Ż ID '{storedTargetPortalID}'�� ã�� �� �����ϴ�! �÷��̾� ��ġ ����.");
        }

        // PlayerPrefs ������ ���� (������ ������ �� �� ��� �� ����)
        PlayerPrefs.DeleteKey("TargetPortalIDAfterSceneLoad");
        PlayerPrefs.Save();

        // �̵� �Ϸ� �� �÷��� ���� �� �÷��̾� Ȱ��ȭ
        FinishTeleportation();
    }

    void FinishTeleportation()
    {
        // 1. ȭ�� ���̵� �� (�ɼ�)
        // ���� ��� ScreenFader.Instance.FadeIn(fadeDuration);
        Debug.Log("ȭ�� ���̵� �� ��...");
        // yield return new WaitForSeconds(fadeDuration); // �����δ� ���̵� ���� �Ϸ�� ������ ��ٸ�

        // 2. �÷��̾� ������ Ȱ��ȭ
        SetPlayerMovement(true);

        // 3. �ڷ���Ʈ �÷��� ����
        isTeleporting = false;
        playerInRange = false; // OnTriggerExit ���̵� �ٽ� �� �� �ֵ���
        Debug.Log("�ڷ���Ʈ �Ϸ�.");
    }

    void SetPlayerMovement(bool enabled)
    {
        if (player != null)
        {
            // �÷��̾� �������� �����ϴ� ��ũ��Ʈ�� �ִٸ� �ش� ��ũ��Ʈ�� Ȱ��ȭ�� ����
            // ��:
            // var controller = player.GetComponent<PlayerController>(); // �������� �÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ
            // if (controller != null) controller.enabled = enabled;

            // �Ǵ� Rigidbody�� ����ϴ� ���
            // var rb = player.GetComponent<Rigidbody>();
            // if (rb != null) rb.isKinematic = !enabled; // Kinematic���� �����ϸ� ������ �̵� �Ұ�

            Debug.Log($"�÷��̾� ������ {(enabled ? "Ȱ��ȭ" : "��Ȱ��ȭ")}.");
        }
    }

    // ����Ƽ �����Ϳ��� ��Ż�� �������� �ð�ȭ
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale); // ��Ż ����
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * 2f); // ��Ż ������ ǥ��
        Gizmos.DrawSphere(transform.position + transform.forward * 2f, 0.2f); // ������ ����
    }
}