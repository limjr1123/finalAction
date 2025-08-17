using System.Collections.Generic;
using GameSave;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [Header("Database")]
    [SerializeField] private QuestDatabase questDatabase;
    public QuestDatabase GetQuestDataBase => questDatabase;


    [Header("Active Quests (runtime)")]
    private List<QuestProgress> activeQuests = new List<QuestProgress>();



    void Start()
    {
        // QuestData startQuest = questDatabase.GetQuestByID("TalkToChief1");
        // if (startQuest != null)
        // {
        //     Debug.Log(startQuest.title + " 퀘스트 등록!");
        //     AddQuest(startQuest);
        // }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameEvents.NPCTalked("Chief");
        }
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
        GameEvents.OnNPCTalked += OnNPCTalked;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyKilled -= OnEnemyKilled;
        GameEvents.OnItemGet -= OnItemGet;
        GameEvents.OnDungeonReach -= OnDungeonReach;
        GameEvents.OnDungeonClear -= OnDungeonClear;
        GameEvents.OnNPCTalked -= OnNPCTalked;
    }


    public void AddQuest(QuestData questData)
    {
        // 중복 등록 방지
        if (GetProgressByID(questData.questID) != null) return;
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

    public void CompleteQuest(QuestProgress progress)
    {
        Debug.Log(progress.questData.title + " 완료!");
        progress.isCompleted = true;

        // foreach (var r in progress.questData.rewards)
        // {
        //     switch (r.type)
        //     {
        //         case RewardType.Gold:
        //             GoldSystem.Instance.AddCurrency(r.amount);
        //             Debug.Log($"+{r.amount} Gold 획득!");
        //             break;

        //         case RewardType.Exp:
        //             PlayerStats.Instance.AddExp(r.amount);
        //             Debug.Log($"+{r.amount} Exp 획득!");
        //             break;

        //         case RewardType.Item:
        //             if (!string.IsNullOrEmpty(r.itemId))
        //             {
        //                 InventoryManager.Instance.AddItem(r.itemId, r.amount);
        //                 Debug.Log($"{r.itemId} x{r.amount} 획득!");
        //             }
        //             break;
        //     }
        // }

        if (!string.IsNullOrEmpty(progress.questData.nextQuestID))
        {
            var next = questDatabase.GetQuestByID(progress.questData.nextQuestID);
            if (next != null)
            {
                AddQuest(next);
                Debug.Log(next.title + " 퀘스트 등록!");
            }
        }
    }


    #endregion



    #region 이벤트 → 업데이트 매핑

    private void OnEnemyKilled(string enemyID) => UpdateObjective(enemyID, ObjectiveType.Kill);
    private void OnItemGet(string itemID, int amount = 1) => UpdateObjective(itemID, ObjectiveType.Collect, amount);
    private void OnDungeonReach(string dungeonID) => UpdateObjective(dungeonID, ObjectiveType.Reach);
    private void OnDungeonClear(string dungeonID) => UpdateObjective(dungeonID, ObjectiveType.Clear);
    private void OnNPCTalked(string npcID) => UpdateObjective(npcID, ObjectiveType.Talk, 1);

    #endregion



    #region 편의 메서드 (외부에서 조회)

    public QuestProgress GetProgressByID(string questID)
    {
        return activeQuests.Find(q => q.questData.questID == questID);
    }

    #endregion

    #region 퀘스트 데이터 저장 및 로드

    public QuestSaveData SaveQuestData()
    {
        QuestSaveData questSaveData = new QuestSaveData();
        // QuestID로 저장
        questSaveData.completedQuests = new List<string>();
        questSaveData.activeQuests = new List<string>();

        // 만약 완료된 퀘스트도 저장한다면 똑같이 하기.
        foreach (var questProgress in activeQuests)
        {
            questSaveData.activeQuests.Add(questProgress.questData.questID);
        }


        return questSaveData;
    }

    public void LoadQuestData(QuestSaveData questSaveData)
    {
        activeQuests.Clear();

        foreach (var questID in questSaveData.activeQuests)
        {
            var questData = questDatabase.GetQuestByID(questID);
            if (questData != null)
            {
                AddQuest(questData);
            }
        }

    }

    #endregion
}