using UnityEngine;

[CreateAssetMenu(fileName = "JobData", menuName = "Scriptable Objects/JobData")]
public class JobData : ScriptableObject
{
    public string jobName;
    public Sprite jobIcon;
    public int baseHP;
    public int baseMP;
    public int baseAttack;
    public int baseDefance;

    // 추가 스탯
}

