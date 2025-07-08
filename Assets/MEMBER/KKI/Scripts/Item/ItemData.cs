using UnityEngine;

public enum ItemType { Weapon, Armor, Consumable }

public abstract class ItemData : ScriptableObject
{
    [Header("아이템 기본 정보")]
    [SerializeField] private string itemID;         // 아이템 식별자
    [SerializeField] private string itemName;       // 아이템 이름
    [SerializeField] private ItemType itemType;     // 아이템 유형
    [SerializeField] private Sprite itemSp;         // 아이템 스프라이트
    [SerializeField] private bool stackable;        // 아이템이 적재될 수 있는가?
    [SerializeField] private int maxStack;          // 아이템 최대 적재 개수
    [TextArea]
    [SerializeField] private string description;    // 아이템 설명서

    public string ItemID => itemID != null ? itemID : "NoID";
    public string ItemName => itemName != null ? itemName : "Unknown";
    public ItemType ItemType => itemType;
    public Sprite Sprite => itemSp;
    public bool Stackable => stackable;
    public int MaxStack => maxStack;
    public string Description => description;
}
