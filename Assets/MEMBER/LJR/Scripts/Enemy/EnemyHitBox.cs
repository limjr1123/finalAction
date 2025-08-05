using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    private EnemyController enemyController;

    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // HitBox와 충돌했을 때 호출되는 메서드.
        if (other.CompareTag("Player"))
        {
            int finalDamage = GetComponentInParent<EnemyStat>().attackDamageRange.GetRandomDamage();
            other.GetComponent<PlayerStats>()?.TakePhysicalDamage(finalDamage);
        }
        else if (other.CompareTag("PlayerGuard") || other.CompareTag("PlayerParry"))
        {
            Debug.Log("패링 성공");
            enemyController.stateMachine.ChangeState(enemyController.stateDict[EnemyStates.GetHit]);
        }
    }
}
