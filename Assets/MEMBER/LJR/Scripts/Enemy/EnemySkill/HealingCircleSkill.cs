using Unity.VisualScripting;
using UnityEngine;

public class HealingCircleSkill : MonoBehaviour, EnemySkillInterface
{
    string SkillName = "HealingCircle";
    [SerializeField] private int Amount = 10; // 회복량
    [SerializeField] private float Radius = 5f; // 회복 범위
    [SerializeField] private float Duration = 5f; // 회복 지속 시간
    [SerializeField] private float cooldown = 3f;
    private float lastUsedTime;

    [SerializeField] private GameObject healingEffectPrefab; // 회복 이펙트 프리팹

    public void Execute()
    {
        
    }

    public bool CanUse() => Time.time >= lastUsedTime + cooldown;
    
    public float GetCooldown() => cooldown;

    public string GetSkillName() => SkillName;

    public void UseSkill(Transform _target)
    {
        if (!CanUse())
            return;
        Debug.Log("스킬사용완");
        GetCooldown();
        lastUsedTime = Time.time;
        GameObject obj = Instantiate(healingEffectPrefab, _target.position, Quaternion.identity); // 회복 이펙트 생성
        obj.GetComponent<HealingCircle>().Initialize(Amount, Radius, Duration);


        Destroy(obj, Duration); // 일정 시간 후 오브젝트 제거
    }
}
