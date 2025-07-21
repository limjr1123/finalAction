using UnityEngine;

[CreateAssetMenu(fileName = "JobData", menuName = "Scriptable Objects/JobData")]
public class JobData : ScriptableObject
{
    public string jobID;
    public string jobName;
    public int baseHP;
    public int baseMP;
    public int baseAttack;
    public int baseDefance;

    // 추가 스탯
}

