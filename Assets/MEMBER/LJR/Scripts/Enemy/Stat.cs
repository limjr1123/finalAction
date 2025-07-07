using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Stat
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;

    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    // 초기값 설정 메서드
    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }


    // 추가 능력치 관리 메서드
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }

}
