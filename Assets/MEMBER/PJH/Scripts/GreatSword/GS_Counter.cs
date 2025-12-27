using UnityEngine;

public class GS_Counter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            EnemyController enemyController = other.GetComponentInParent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.stateMachine.ChangeState(enemyController.stateDict[EnemyStates.GetHit]);
            }

            Debug.Log("»ó¼â ¼º°ø");
        }
    }
}
