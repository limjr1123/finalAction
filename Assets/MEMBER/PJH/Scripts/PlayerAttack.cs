using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 관련")]
    public float comboTime = 1f; // 다음 콤보 입력 대기 시간
    private int comboCount = 0; // 현재 콤보 카운트
    private float lastAttackTime = 0f; // 마지막 공격 입력 시간

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

        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽
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