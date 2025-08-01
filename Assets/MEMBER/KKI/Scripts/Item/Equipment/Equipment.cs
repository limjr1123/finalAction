using UnityEngine;

public enum EquipType
{
    Head,
    Body,
    Gloves,
    Pants,
    Boots,
    Weapon
}

public class Equipment : ItemData, IEquipable
{
    public EquipType equipType;

    // 추가 속성 더하기 (ex. 공격력, 방어럭, 치명타 확률 등등)
    public float attack;
    public float defence;

    public void Equip(GameObject user)
    {
        // 1. 유저 컴포넌트 가져오고
        // user.GetComponent<>();
        // 2. 아이템 장착 함수 호출 (원래 끼고 있는 장비가 있으면 Unequip함수 호출)
    }
    public void Unequip(GameObject user)
    {
        // 1. 유저 컴포넌트 가져오고
        // user.GetComponent<>();
        // 2. 아이템 해제 함수 호출
    }
}