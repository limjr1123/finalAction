using UnityEngine;

public enum EquipType
{
    Weapon,
    Body,
    Accessory,
}
[CreateAssetMenu(fileName = "Equipmentata", menuName = "Scriptable Objects/Item/Equipment")]
public class EquipmentItemData : ItemData, IEquipable
{
    [SerializeField] private EquipType equipType;

    // 추가 속성 더하기 (ex. 공격력, 방어럭, 치명타 확률 등등)
    [SerializeField] private float attack;
    [SerializeField] private float defence;

    public EquipType EquipType => equipType;
    public float Attack => attack;
    public float Defence => defence;

    public void Equip(GameObject user)
    {
        // 1. 유저 컴포넌트 가져오고
        // user.GetComponent<>();
        // 2. 아이템 장착 함수 호출 (원래 끼고 있는 장비가 있으면 Unequip함수 호출)
        // 3. 아이템 장착
    }
    public void Unequip(GameObject user)
    {
        // 1. 유저 컴포넌트 가져오고
        // user.GetComponent<>();
        // 2. 아이템 해제 함수 호출
    }
}