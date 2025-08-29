using UnityEngine;

// 나중에 상점 NPC 등으로 파생하려면 그대로 상속 가능
public class NPC : MonoBehaviour, INPCInteractable
{
    [SerializeField] private NPCData npcData;
    [SerializeField] private string startDialogueID;
    private bool isInteract;

    public virtual void Interact()
    {
        if (isInteract) return;
        isInteract = true;

        // 1) 먼저 '현재 상태 기준'으로 대사를 선택하고 시작
        string id = !string.IsNullOrEmpty(startDialogueID)
            ? startDialogueID
            : GetDialogueBasedOnQuestStatus();

        Debug.Log(GetDialogueBasedOnQuestStatus());
        DialogueManager.Instance.StartDialogue(id, this);

        // 2) 그 다음에 '말 걸었다' 이벤트로 Talk 목표 처리
        if (npcData != null && !string.IsNullOrEmpty(npcData.npcID))
        {
            GameEvents.NPCTalked(npcData.npcID);
        }


        // 3) 마지막으로 (촌장 등) 규칙 평가: 다음 챕터 퀘스트 지급 등
        var giver = GetComponent<QuestGiverOnTalk>();
        if (giver != null) giver.Evaluate();
    }


    public virtual void EndInteraction()
    {
        if (!isInteract) return;
        isInteract = false;
    }

    // 필요시 퀘스트 상태에 맞춘 대화 선택
    protected virtual string GetDialogueBasedOnQuestStatus()
    {
        if (npcData == null || npcData.dialogueID == null || npcData.dialogueID.Length == 0)
            return startDialogueID;

        foreach (var id in npcData.dialogueID)
        {
            var data = DialogueManager.Instance.GetDialogueData(id);
            if (data == null) continue;

            bool ok = IsQuestConditionMet(data);
            // Debug.Log($"[DialoguePick] id={id}, reqQuest={data.requiredQuestID}, reqStatus={data.requiredQuestStatus}, pass={ok}");
            if (ok) return id;
        }

        return npcData.dialogueID[0];
    }

    private bool IsQuestConditionMet(DialogueData data)
    {
        if (string.IsNullOrEmpty(data.requiredQuestID) ||
            data.requiredQuestStatus == DialogueQuestStatus.None)
            return true; // 조건이 없으면 항상 노출

        var qm = QuestManager.Instance;
        var progress = qm.GetProgressByID(data.requiredQuestID);

        switch (data.requiredQuestStatus)
        {
            case DialogueQuestStatus.NotAccepted:
                return progress == null;
            case DialogueQuestStatus.InProgress:
                return progress != null && !progress.isCompleted;
            case DialogueQuestStatus.Completed:
                return progress != null && progress.isCompleted;
            default:
                return true;
        }
    }
    public string GetNPCId() => npcData != null ? npcData.npcID : string.Empty;
}
