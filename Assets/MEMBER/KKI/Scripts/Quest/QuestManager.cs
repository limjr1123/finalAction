using System.Collections.Generic;
using GameSave;
using JetBrains.Annotations;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField] private QuestDatabase questDatabase;
    private List<QuestProgress> activeQuests = new List<QuestProgress>();


    void Start()
    {
        QuestData questData = questDatabase.GetQuestByID("Dungeon1Reach");
        if (questData != null)
        {
            Debug.Log(questData.title + "퀘스트 등록!");
            AddQuest(questData);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameEvents.DungeonReach("Dungeon1");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            GameEvents.DungeonClear("Dungeon1");
        }
    }
    private void OnEnable()
    {
        GameEvents.OnEnemyKilled += OnEnemyKilled;
        GameEvents.OnItemGet += OnItemGet;
        GameEvents.OnDungeonReach += OnDungeonReach;
        GameEvents.OnDungeonClear += OnDungeonClear;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyKilled -= OnEnemyKilled;
        GameEvents.OnItemGet -= OnItemGet;
        GameEvents.OnDungeonReach -= OnDungeonReach;
        GameEvents.OnDungeonClear -= OnDungeonClear;
    }


    public void AddQuest(QuestData questData)
    {
        activeQuests.Add(new QuestProgress(questData));
    }

    #region 퀘스트 업데이트 함수

    public void UpdateObjective(string targetId, ObjectiveType type, int amount = 1)
    {
        for (int q = 0; q < activeQuests.Count; q++)
        {
            var prograss = activeQuests[q];
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

                    if (IsQuestCompleted(prograss))
                    {
                        CompleteQuest(prograss);
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

    public void CompleteQuest(QuestProgress prograss)
    {
        Debug.Log(prograss.questData.title + "완료!");
        // 1. 완료 퀘스트 등록하기
        prograss.isCompleted = true;

        // 2. 퀘스트 보상 받기
        // prograss.questData.rewards;

        // 3. 다음 퀘스트 있으면 자동 등록
        if (!string.IsNullOrEmpty(prograss.questData.nextQuestID))
        {
            QuestData questData = questDatabase.GetQuestByID(prograss.questData.nextQuestID);
            if (questData != null)
            {
                AddQuest(questData);
                Debug.Log(questData.title + "퀘스트 등록!");
            }

        }
    }

    #endregion



    #region 퀘스트 업데이트 등록 함수
    private void OnEnemyKilled(string enemyID)
    {
        UpdateObjective(enemyID, ObjectiveType.Kill);
    }

    private void OnItemGet(string itemID, int amount = 1)
    {
        UpdateObjective(itemID, ObjectiveType.Collect, amount);
    }

    private void OnDungeonReach(string dungeonID)
    {
        UpdateObjective(dungeonID, ObjectiveType.Reach);
    }

    private void OnDungeonClear(string dungeonID)
    {
        UpdateObjective(dungeonID, ObjectiveType.Clear);
    }


    #endregion


    #region 퀘스트 데이터 저장 및 로드

    public QuestSaveData SaveQuestData()
    {
        QuestSaveData questSaveData = new QuestSaveData();
        // QuestID로 저장
        questSaveData.completedQuests = new List<string>();
        questSaveData.currentQuests = new List<string>();



        return questSaveData;
    }

    public void LoadQuestData(QuestSaveData questSaveData)
    {

    }

    #endregion
}