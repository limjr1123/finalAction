using UnityEngine;

public class EnemyGetHitState : EnemyState<EnemyController>
{
    Collider hitBox;

    private void Awake()
    {
        hitBox = GetComponent<Collider>();
        if (hitBox == null)
        {
            Debug.LogError("EnemyGetHitState: HitBox Collider가 없습니다.");
        }
    }

    public override void Enter(EnemyController owner)
    {

    }

}
