using UnityEngine;

public class BowController : MonoBehaviour
{
    public Animator anim { get; set; } // 무기 애니메이터
    public Transform bowAim; // 활의 조준 위치
    public Transform arrowSpawnPoint; // 화살이 생성될 위치
    public GameObject arrowPrefab; // 생성할 화살 프리팹

    public Vector3 direction; // 화살의 방향 벡터

    void Awake()
    {
        anim = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
    }

    public void DrawBow()
    {
        anim.SetBool("Draw", true);
        anim.SetBool("Shoot", false);

        // 생성 지점에서 조준점으로의 방향 벡터 계산
        Vector3 direction = (bowAim.position - arrowSpawnPoint.position).normalized;

        // 방향 벡터를 회전값으로 변환
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, targetRotation, arrowSpawnPoint); // 화살 생성
    }

    public void ShootArrow()
    {
        anim.SetBool("Draw", false);
        anim.SetBool("Shoot", true);


    }

}
