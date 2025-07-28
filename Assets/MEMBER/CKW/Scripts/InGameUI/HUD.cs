using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Skiil Buttons")]
    [SerializeField] Button skill_1;
    [SerializeField] Button skill_2;
    [SerializeField] Button skill_3;
    [SerializeField] Button skill_4;
    [SerializeField] Button attackButton;


    [Header("Consume Quick Slot")]
    [SerializeField] Button consume_1;
    [SerializeField] Button consume_2;




    void Start()
    {
        if (skill_1 != null)
            skill_1.onClick.AddListener(OnSkillButton1);
        if (skill_2 != null)
            skill_2.onClick.AddListener(OnSkillButton2);
        if (skill_3 != null)
            skill_3.onClick.AddListener(OnSkillButton3);
        if (skill_4 != null)
            skill_4.onClick.AddListener(OnSkillButton4);
        if (attackButton != null)
            attackButton.onClick.AddListener(OnAttackButton);



        if (consume_1 != null)
            consume_1.onClick.AddListener(OnConsumeButton1);
        if (consume_2 != null)
            consume_2.onClick.AddListener(OnConsumeButton2);
    }

    // 스킬 & 공격 버튼 눌렀을 때
    private void OnSkillButton1()
    {

    }


    private void OnSkillButton2()
    {

    }


    private void OnSkillButton3()
    {

    }


    private void OnSkillButton4()
    {

    }


    private void OnAttackButton()
    {

    }

    // 퀵슬롯(소모품) 눌렀을 때
    private void OnConsumeButton1()
    {

    }


    private void OnConsumeButton2()
    {

    }
}
