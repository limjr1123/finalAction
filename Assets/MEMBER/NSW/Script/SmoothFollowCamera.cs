using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("ī�޶� ���� �÷��̾� ������Ʈ�� �Ҵ��ϼ���.")]
    public Transform target; // ī�޶� ���� �÷��̾� Transform (Inspector���� �Ҵ�)

    [Header("Follow Settings")]
    [Tooltip("�÷��̾�κ��� ī�޶� ������ �Ÿ��Դϴ�.")]
    public Vector3 offset = new Vector3(0f, 2f, -5f); // �÷��̾�κ����� ������� ��ġ (Y�� 2m ��, Z�� -5m ��)
    [Tooltip("ī�޶� ��ǥ ��ġ�� �����ϴ� �ӵ� (�������� ������ ����).")]
    public float smoothSpeed = 0.125f; // ī�޶� �������� �ε巯�� ���� (�������� �ε巯�� = ����)

    [Header("Look At Settings")]
    [Tooltip("ī�޶� �ٶ� ��ǥ ������ ������ (�÷��̾��� �� ����).")]
    public Vector3 lookAtOffset = new Vector3(0f, 1f, 0f); // ī�޶� �÷��̾ �ٶ� �������� ������ (��: �÷��̾��� ��ġ ����)

    void Start()
    {
        // target�� �Ҵ���� �ʾҴٸ� "Player" �±׸� ���� ������Ʈ�� ã���ϴ�.
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogError("SmoothFollowCamera: 'Player' �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�. target�� �÷��̾ �Ҵ����ּ���.");
                this.enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
                return;
            }
        }
    }

    // FixedUpdate�� ���� ����̳� �ε巯�� ī�޶� �̵��� �����մϴ�.
    // LateUpdate�� ��� Update �Լ��� ����� �� ȣ��ǹǷ�, �÷��̾� �̵� �� ī�޶� ���󰡰� �� �� �����մϴ�.
    void LateUpdate()
    {
        if (target == null) return;

        // ��ǥ ��ġ ���: �÷��̾� ��ġ + ������
        Vector3 desiredPosition = target.position + offset;

        // ���� ī�޶� ��ġ���� ��ǥ ��ġ�� �ε巴�� �̵�
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // ī�޶� �ٶ� ��ǥ ���� ���: �÷��̾� ��ġ + �ٶ� ������
        Vector3 lookAtTarget = target.position + lookAtOffset;

        // ī�޶� ��ǥ ������ �ٶ󺸵��� ����
        transform.LookAt(lookAtTarget);
    }

    // ����Ƽ �����Ϳ��� ������ ī�޶� �ٶ� ��ġ�� �ð�ȭ
    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            // ī�޶� �ٶ� �÷��̾��� ��ǥ ����
            Vector3 lookAtTargetPosition = target.position + lookAtOffset;
            Gizmos.DrawSphere(lookAtTargetPosition, 0.2f); // ���� ���� ǥ��

            Gizmos.color = Color.yellow;
            // ī�޶��� ���� ��ġ�� ��ǥ ��ġ�� �����ϴ� ��
            Gizmos.DrawLine(transform.position, lookAtTargetPosition);

            Gizmos.color = Color.green;
            // �÷��̾�κ��� ī�޶� ���� desiredPosition
            Vector3 desiredPosFromPlayer = target.position + offset;
            Gizmos.DrawSphere(desiredPosFromPlayer, 0.1f); // ���� ���� ǥ��
            Gizmos.DrawLine(target.position, desiredPosFromPlayer); // �÷��̾�� ī�޶� ��ǥ ��ġ ����
        }
    }
}