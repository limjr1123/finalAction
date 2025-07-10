using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public int Damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>(); // �������̽� �پ��ִ��� Ȯ��
            if (damageable != null)
            {
                damageable.TakeDamage(Damage);
            }
        }
    }
}
