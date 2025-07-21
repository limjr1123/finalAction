using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    public List<QuestProgress> activeQuests = new List<QuestProgress>();

    public void AddQuest(QuestData questData)
    {
        activeQuests.Add(new QuestProgress(questData));
    }

    public void UpdateObjective(string targetId, ObjectiveType type, int amount = 1)
    {
        foreach (var prograss in activeQuests)
        {
            for (int i = 0; i < prograss.questData.questObjectives.Length; i++)
            {
                var obj = prograss.questData.questObjectives[i];
                if (!prograss.isCompleted &&
                    obj.type == type &&
                    obj.targetId == targetId)
                {

                }
            }
        }
    }
}