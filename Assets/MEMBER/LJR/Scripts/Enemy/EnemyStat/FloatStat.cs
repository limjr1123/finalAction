using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatStat
{
    [SerializeField] private float baseValue;

    public List<float> modifiers;

    public float GetValue()
    {
        float finalValue = baseValue;

        foreach (float modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    // 초기값 설정 메서드
    public void SetDefaultValue(float _value)
    {
        baseValue = _value;
    }


    // 추가 능력치 관리 메서드
    public void AddModifier(float _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(float _modifier)
    {
        modifiers.Remove(_modifier);
    }
}