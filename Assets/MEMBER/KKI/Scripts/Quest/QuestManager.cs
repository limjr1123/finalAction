using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private void OnEnable()
    {
        GameEvents.OnEnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyKilled -= OnEnemyKilled;
    }

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
                    prograss.currentAmounts[i] += amount;
                    if (prograss.currentAmounts[i] >= obj.targetAmount)
                        prograss.currentAmounts[i] = obj.targetAmount;

                    // 목표 전체 달성 체크
                    if (IsQuestCompleted(prograss))
                    {
                        prograss.isCompleted = true;
                    }
                }
            }
        }
    }

    public bool IsQuestCompleted(QuestProgress progress)
    {
        for (int i = 0; i < progress.questData.questObjectives.Length; i++)
        {
            if (progress.currentAmounts[i] < progress.questData.questObjectives[i].targetAmount)
                return false;
        }

        return true;
    }

    private void OnEnemyKilled(string enemyId)
    {
        UpdateObjective(enemyId, ObjectiveType.Kill);
    }
}