using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public int Damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>(); // 인터페이스 붙어있는지 확인
            if (damageable != null)
            {
                damageable.TakeDamage(Damage);
            }
        }
    }
}
