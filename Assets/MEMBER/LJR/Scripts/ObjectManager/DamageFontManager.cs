using UnityEngine;
using System.Collections.Generic;

public class DamageFontManager : MonoBehaviour
{
    public static DamageFontManager Instance;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private int poolSize = 20;
    private List<GameObject> damageTextPool = new List<GameObject>();
    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }


        canvas = GetComponentInParent<Canvas>();
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject textObj = Instantiate(damageTextPrefab, canvas.transform);
            textObj.SetActive(false);
            damageTextPool.Add(textObj);
        }
    }

    public void ShowDamage(int damage, Transform target, bool isCritical = false, bool isHeal = false)
    {
        GameObject textObj = damageTextPool.Find(obj => !obj.activeInHierarchy);
        if (textObj == null)
        {
            textObj = Instantiate(damageTextPrefab, canvas.transform);
            damageTextPool.Add(textObj);
        }

        DamageText damageText = textObj.GetComponent<DamageText>();
        damageText.Initialize(damage, target, Vector3.up * 1f, isCritical, isHeal);
    }
}