using UnityEngine;
using System.Collections; // �ڷ�ƾ ����� ���� �ʿ�

public class TeleportPad : MonoBehaviour
{
    [Header("Teleport Pad Settings")]
    [Tooltip("�� �ڷ���Ʈ �е��� ���� ID (���� ����, ���������θ� ���).")]
    public string padID = "TeleportPad_A";

    [Header("Destination Settings")]
    [Tooltip("�÷��̾ �̵��� ������ Transform�� �Ҵ��ϼ���.")]
    public Transform destinationTransform; // �÷��̾ �̵��� ������ ��ġ �� ȸ��

    [Tooltip("������ Ʈ�������� �������� �÷��̾ ������ �Ÿ�.")]
    public float spawnOffset = 1.5f; // ������ Ʈ�������� �������� ������ �Ÿ�

    [Header("Player Settings")]
    public GameObject player; // �÷��̾� ������Ʈ (Inspector���� �Ҵ� �Ǵ� "Player" �±׷� ã��)
    public float teleportDelay = 0.5f; // �÷��̾ �е忡 �� �� �̵������� ������
    public float fadeDuration = 0.5f; // ȭ�� ���̵� ��/�ƿ� �ð� (�ɼ�, ���� ������ ���� ��ũ��Ʈ ����)

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
                Debug.LogError("TeleportPad: Player GameObject not found. Please assign it in the Inspector or tag your player with 'Player'.");
            }
        }

        // �ڷ���Ʈ �е� ������Ʈ�� Collider�� ������ ���
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("TeleportPad: This TeleportPad needs a Collider component with 'Is Trigger' enabled to detect player entry.", this);
        }

        // ������ Transform�� �������� �ʾ����� ���
        if (destinationTransform == null)
        {
            Debug.LogError($"TeleportPad '{padID}': Destination Transform is not set. This pad cannot function!", this);
            this.enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
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
                Debug.Log($"�÷��̾� '{player.name}'�� �ڷ���Ʈ �е� '{padID}'�� �����߽��ϴ�. �̵� �õ�.");
                StartCoroutine(PerformTeleport());
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
                Debug.Log($"�÷��̾� '{player.name}'�� �ڷ���Ʈ �е� '{padID}'���� ������ϴ�.");
            }
        }
    }

    IEnumerator PerformTeleport()
    {
        // 1. �ڷ���Ʈ ������
        yield return new WaitForSeconds(teleportDelay);

        // 2. ȭ�� ���̵� �ƿ� (�ɼ�)
        // ���� ���ӿ����� ScreenFader ���� ���� ��ũ��Ʈ�� ����մϴ�.
        // ��: ScreenFader.Instance.FadeOut(fadeDuration);
        Debug.Log("ȭ�� ���̵� �ƿ� ��...");
        yield return new WaitForSeconds(fadeDuration); // ���̵� �ƿ��� �Ϸ�� ������ ��ٸ� (�ӽ�)

        // �÷��̾� ������ ��Ȱ��ȭ (���� �̵� �� �̵� ����)
        SetPlayerMovement(false);

        // 3. �÷��̾� ��ġ �� ȸ�� ����
        if (destinationTransform != null)
        {
            Vector3 destination = destinationTransform.position + destinationTransform.forward * spawnOffset;
            player.transform.position = destination;
            player.transform.rotation = destinationTransform.rotation;
            Debug.Log($"�÷��̾ '{destinationTransform.name}' (Pad ID: {padID}) ��ġ�� �̵����׽��ϴ�: {destination}");
        }
        else
        {
            Debug.LogError($"TeleportPad '{padID}': ������ Transform�� Null�Դϴ�. �̵� ����!");
        }

        // 4. �̵� �Ϸ� �� ���̵� �� �� �÷��� ����
        StartCoroutine(FinishTeleportation());
    }

    IEnumerator FinishTeleportation()
    {
        // 1. ȭ�� ���̵� �� (�ɼ�)
        // ��: ScreenFader.Instance.FadeIn(fadeDuration);
        Debug.Log("ȭ�� ���̵� �� ��...");
        yield return new WaitForSeconds(fadeDuration); // ���̵� ���� �Ϸ�� ������ ��ٸ� (�ӽ�)

        // 2. �÷��̾� ������ Ȱ��ȭ
        SetPlayerMovement(true);

        // 3. �ڷ���Ʈ �÷��� ����
        isTeleporting = false;
        playerInRange = false;
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

    // ����Ƽ �����Ϳ��� �ڷ���Ʈ �е�� �������� �ð�ȭ
    void OnDrawGizmos()
    {
        // ���� �е� ���� ǥ��
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        // ������ Ʈ�������� �Ҵ�Ǿ� �ִٸ� ������ ǥ��
        if (destinationTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(destinationTransform.position, 0.5f); // ������ ����
            Gizmos.DrawRay(destinationTransform.position, destinationTransform.forward * 2f); // ������ ������
            Gizmos.DrawSphere(destinationTransform.position + destinationTransform.forward * spawnOffset, 0.3f); // ���� ����

            // ���� �е�� �������� �����ϴ� ��
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destinationTransform.position);
        }
    }
}