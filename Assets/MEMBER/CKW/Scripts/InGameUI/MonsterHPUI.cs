using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPUI : MonoBehaviour
{
    public static MonsterHPUI Instance;


    [SerializeField] Image hpSlider;
    [SerializeField] TextMeshProUGUI monsterName;
    [SerializeField] float hideDelay = 5f;

    private EnemyStat currentTarget;
    private float lastDamageTime;


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
    }


    public void SetTarget(EnemyStat monster)
    {
        currentTarget = monster;
        monsterName.text = monster.name;
        gameObject.SetActive(true);
        UpdateHP(monster.currentHealth);
        lastDamageTime = Time.time;
    }


    public void UpdateHP(float currentHP)
    {
        if (currentTarget == null) return;

        hpSlider.fillAmount = currentHP / 100;
        lastDamageTime = Time.time;
    }


    public void HideUI()
    {
        currentTarget = null;
        gameObject.SetActive(false);
    }
}
