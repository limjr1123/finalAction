using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPUI : MonoBehaviour
{
    public static MonsterHPUI Instance;


    [SerializeField] Image hpSlider;
    [SerializeField] TextMeshProUGUI monsterName;
    [SerializeField] GameObject hpPanel;
    [SerializeField] float hideDelay = 5f;


    private EnemyStat currentTarget;
    private float lastDamageTime;
    private float targetMaxHP;

    void Awake()
    {
        if (Instance != null)
            Instance = this;
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }


    private void Update()
    {
        if (currentTarget == null) return;

        if (Time.time - lastDamageTime > hideDelay)
        {
            HideUI();
        }


        if (currentTarget.currentHealth <= 0)
        {
            HideUI();
        }
        else
        {
            UpdateHP(currentTarget.currentHealth, targetMaxHP);
        }
    }


    public void SetTarget(EnemyStat monster)
    {
        currentTarget = monster;
        monsterName.text = monster.enemyName;
        hpPanel.SetActive(true);
        
        targetMaxHP = monster.maxHealth.GetValue();

        UpdateHP(monster.currentHealth, monster.maxHealth.GetValue());
        lastDamageTime = Time.time;
        
    }


    public void UpdateHP(float currentHP, float maxHP)
    {
        if (currentTarget == null) return;

        hpSlider.fillAmount = currentHP / maxHP;
        lastDamageTime = Time.time;
    }


    public void HideUI()
    {
        currentTarget = null;
        hpPanel.SetActive(false);
    }
}
