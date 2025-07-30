using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class QuestDatas
{
    public string desription;
    public QuestData questData;
}

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Scriptable Objects/Database/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField] private List<QuestDatas> quests = new();
    private Dictionary<string, QuestDatas> questDict;

    public void Init()
    {
        questDict = quests.ToDictionary(quests => quests.questData.questID);
    }

    public QuestData GetQuestByID(string id)
    {
        if (questDict == null)
            Init();
        return questDict.TryGetValue(id, out var quest) ? quest.questData : null;
    }
}
