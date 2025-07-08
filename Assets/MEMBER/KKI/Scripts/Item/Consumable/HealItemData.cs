using UnityEngine;

[CreateAssetMenu(fileName = "HealItemData", menuName = "Scriptable Objects/Item/Consumable/HealItem")]
public class HealItemData : ItemData, IUsable
{
    [Header("힐 아이템 기본 정보")]
    [SerializeField] private float healValue;


    // 프로퍼티
    public float HealValue => healValue;

    public void Use(GameObject user)
    {
        // user.GetComponent<Player>().Heal(healValue);
    }
}
