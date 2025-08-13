using System.Collections;
using UnityEngine;

public class SwordWindController : MonoBehaviour
{
    [SerializeField] private Vector3 vectorDir;       // 바람의 방향(Vector3)
    [SerializeField] private Transform transformDir;  // 바람의 방향(Transform)
    [SerializeField] private float zAngle;            // 바람의 Z축 회전 각도
    [SerializeField] private float speed = 15f; // 바람의 속도
    [SerializeField] private float lifeTime = 3f; // 바람의 생존 시간
    [SerializeField] private int damage; // 바람의 데미지
    [SerializeField] GameObject windWay;



    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }


    public void InitializeSwordWind(Transform targetTransform, float _zAngle, int _damage)
    {
        SwordWindInfoSetting(targetTransform, _zAngle);
        SetDirection(transformDir, zAngle);
        SetDamage(_damage);
        StartCoroutine(DestroyCoroutine());
    }

    public void SwordWindInfoSetting(Transform _transformDir, float _zAngle)
    {
        transformDir = _transformDir;
        zAngle = _zAngle;
    }

    public void SetDirection(Transform targetTransform, float zAngle)
    {
        // Transform의 z축 방향 (forward) 가져오기
        Vector3 direction = targetTransform.forward.normalized;

        // Vector3.forward에서 direction으로의 회전
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, direction);

        // 최종 회전 적용
        transform.rotation = rotation;
        transform.Rotate(0, 0, zAngle); // z축 회전 적용
    }

    public void SetDamage(int _damage)
    {
        damage = _damage * 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        // HitBox와 충돌했을 때 호출되는 메서드.
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>()?.TakePhysicalDamage(damage);
            HitEffectManager.Instance.EffectCreate(other.transform, HitEffectType.Slash, new Vector3(0, 1f, 0));
        }
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
