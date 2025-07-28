using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    EnemyController enemyController;
    [SerializeField] GameObject weapon; // 원거리 공격에 사용할 무기 오브젝트
    Animator anim;

    public bool inAction { get; private set; } = false; // 현재 공격 동작 중인지 여부  
    public bool inGetHit { get; set; } = false; // 현재 피격 상태인지 여부

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        
    }

}
