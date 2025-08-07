using System;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] PlayerStats target;                // 공격 대상
    [SerializeField] SphereCollider sphereCollider;     // 범위 감지용 콜라이더
    [SerializeField] private float aggroRange;          // 어그로 범위
    public event Action<GameObject> OnTargetDetected;   // 타겟 감지 이벤트

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void SetAggroRange(float range)
    {
        aggroRange = range;
        sphereCollider.radius = aggroRange;
    }   

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
            return;
        target = other.GetComponent<PlayerStats>();
        if (target != null)
            OnTargetDetected?.Invoke(target.gameObject);
    }
}
