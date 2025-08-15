using UnityEngine;

public class QuestGiverOnTalk : MonoBehaviour
{
    [System.Serializable]
    public class GrantRule
    {
        [Tooltip("이 퀘스트가 완료되어 있어야")]
        public string requireCompletedQuestID;

        [Tooltip("이 퀘스트를 새로 준다 (이미 진행 중이면 무시)")]
        public string grantQuestID;
    }

    [Header("안전장치: 특정 NPC만 (예: Chief)")]
    public string npcIdMustBe = "Chief";

    [Header("대화 시 평가할 규칙들")]
    public GrantRule[] rules;

    public void Evaluate()
    {
        var qm = QuestManager.Instance;
        var npc = GetComponent<NPC>();
        if (npc == null) return;

        // 특정 NPC로 제한 (비우면 모든 NPC 허용)
        if (!string.IsNullOrEmpty(npcIdMustBe))
        {
            if (npc.GetNPCId() != npcIdMustBe) return;
        }

        foreach (var r in rules)
        {
            if (string.IsNullOrEmpty(r.grantQuestID)) continue;

            var req = string.IsNullOrEmpty(r.requireCompletedQuestID)
                        ? null
                        : qm.GetProgressByID(r.requireCompletedQuestID);

            // requireCompletedQuestID가 비어있으면 항상 지급
            bool requirementOk = string.IsNullOrEmpty(r.requireCompletedQuestID) ||
                                 (req != null && req.isCompleted);

            var already = qm.GetProgressByID(r.grantQuestID);

            if (requirementOk && already == null)
            {
                var q = qm.GetQuestDataBase.GetQuestByID(r.grantQuestID);
                if (q != null)
                {
                    qm.AddQuest(q);
                    Debug.Log($"[QuestGiverOnTalk] '{r.grantQuestID}' 퀘스트 등록!");
                }
            }
        }
    }
}
