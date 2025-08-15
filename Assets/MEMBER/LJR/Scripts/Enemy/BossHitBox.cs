using UnityEngine;

public class BossHitBox : MonoBehaviour
{
    [SerializeField] private HitEffectType hitEffectType;

    private BossController bossController;

    private void Awake()
    {
        bossController = GetComponentInParent<BossController>();
    }

    void Start()
    {
        //hitEffectType = bossController.meleeBoss.hitEffectType;
    }

    private void OnTriggerEnter(Collider other)
    {
        // HitBox와 충돌했을 때 호출되는 메서드.
        if (other.CompareTag("Player"))
        {
            int finalDamage = GetComponentInParent<EnemyStat>().attackDamageRange.GetRandomDamage();
            other.GetComponent<PlayerStats>()?.TakePhysicalDamage(finalDamage);
            HitEffectManager.Instance.EffectCreate(other.transform, hitEffectType, new Vector3(0, 1f, 0));
        }
        else if (other.CompareTag("PlayerParry"))
        {
            Debug.Log("패링 성공");
            PlayerStateMachine playerStateMachine = other.GetComponentInParent<PlayerStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.OnParrySuccess();
            }
            bossController.stateMachine.ChangeState(bossController.stateDict[BossStates.GetHit]);
        }
        else if (other.CompareTag("PlayerGuard"))
        {
            PlayerStateMachine playerStateMachine = other.GetComponentInParent<PlayerStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.OnGuardSuccess();
                HitEffectManager.Instance.EffectCreate(other.transform, HitEffectType.Block);
            }
        }
    }
}
