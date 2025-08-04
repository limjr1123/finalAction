using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Skiil Buttons")]
    [SerializeField] Button jumpButton;
    [SerializeField] Button evasionButton;
    [SerializeField] Button counterButton;
    [SerializeField] Button attackButton;
    [SerializeField] Button skill_1;
    [SerializeField] Button skill_2;


    [Header("Consume Quick Slot")]
    [SerializeField] Button consume_1;
    [SerializeField] Button consume_2;




    void Start()
    {
        if (jumpButton != null)
            jumpButton.onClick.AddListener(OnJumpButton);
        if (evasionButton != null)
            evasionButton.onClick.AddListener(OnEvasionButton);
        if (counterButton != null)
            counterButton.onClick.AddListener(OnCounterButton);
        if (attackButton != null)
            attackButton.onClick.AddListener(OnAttackButton);
        if (skill_1 != null)
            skill_1.onClick.AddListener(OnSkillButton1);
        if (skill_2 != null)
            skill_2.onClick.AddListener(OnSkillButton2);



        if (consume_1 != null)
            consume_1.onClick.AddListener(OnConsumeButton1);
        if (consume_2 != null)
            consume_2.onClick.AddListener(OnConsumeButton2);
    }

    // 스킬 & 공격 버튼 눌렀을 때
    private void OnJumpButton()
    {

    }


    private void OnEvasionButton()
    {

    }


    private void OnCounterButton()
    {

    }


    private void OnAttackButton()
    {

    }


    private void OnSkillButton1()
    {

    }


    private void OnSkillButton2()
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
