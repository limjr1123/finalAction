using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField] private HitEffectType hitEffectType;
    private Collider hitBoxCollider; // HitBox의 Collider
    private EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
        hitBoxCollider = GetComponent<Collider>();
    }

    void Start()
    {
        hitEffectType = enemyController.meleeEnemy.hitEffectType;
    }

    private void OnTriggerEnter(Collider other)
    {
        // HitBox와 충돌했을 때 호출되는 메서드.
        if (other.CompareTag("Player"))
        {
            hitBoxCollider.enabled = false; // HitBox 비활성화
            int finalDamage = GetComponentInParent<EnemyStat>().attackDamageRange.GetRandomDamage();
            other.GetComponent<PlayerStats>()?.TakePhysicalDamage(finalDamage);
            HitEffectManager.Instance.EffectCreate(other.transform, hitEffectType, new Vector3(0, 1f, 0));
        }
        else if (other.CompareTag("PlayerParry"))
        {
            hitBoxCollider.enabled = false; // HitBox 비활성화
            PlayerStateMachine playerStateMachine = other.GetComponentInParent<PlayerStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.OnParrySuccess();
            }
            enemyController.stateMachine.ChangeState(enemyController.stateDict[EnemyStates.GetHit]);
        }
        else if (other.CompareTag("PlayerGuard"))
        {
            hitBoxCollider.enabled = false; // HitBox 비활성화
            PlayerStateMachine playerStateMachine = other.GetComponentInParent<PlayerStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.OnGuardSuccess();
                HitEffectManager.Instance.EffectCreate(other.transform, HitEffectType.Block);
            }
        }
    }
}
