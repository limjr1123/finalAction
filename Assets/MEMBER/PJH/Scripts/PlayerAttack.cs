using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("���� ����")]
    public float comboTime = 1f; // ���� �޺� �Է� ��� �ð�
    private int comboCount = 0; // ���� �޺� ī��Ʈ
    private float lastAttackTime = 0f; // ������ ���� �Է� �ð�

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (comboCount > 0 && Time.time - lastAttackTime > comboTime)
        {
            ResetCombo();
        }

        if (Input.GetMouseButtonDown(0)) // ���콺 ����
        {
            OnAttackInput();
        }
    }

    void OnAttackInput()
    {
        lastAttackTime = Time.time;

        comboCount = Mathf.Clamp(comboCount + 1, 1, 4);

        if (comboCount == 1)
        {
            anim.SetTrigger("Attack");
        }
        else
        { 
            anim.SetTrigger("NextCombo");
        }

    }


    public void OnLastAttackEnd()
    {
        ResetCombo();
    }

    private void ResetCombo()
    {
        comboCount = 0;
        anim.SetInteger("ComboCount", 0);
    }
}