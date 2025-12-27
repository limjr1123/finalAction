using TMPro;
using UnityEngine;

public class QuestHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questText;

    void OnEnable()
    {
        QuestManager.Instance.OnQuestsUpdated += Refresh;
        Refresh(); // 씬 진입 직후 1회 갱신
    }

    void OnDisable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestsUpdated -= Refresh;
    }

    void Refresh()
    {
        var prog = QuestManager.Instance.GetFirstActiveQuest();
        if (prog == null || prog.isCompleted)
        {
            questName.text = "";
            questText.text = "";
            return;
        }

        // 제목
        questName.text = prog.questData.title;

        // 목표 진행도(여러 개면 줄바꿈)
        System.Text.StringBuilder sb = new();
        var objs = prog.questData.questObjectives;
        for (int i = 0; i < objs.Length; i++)
        {
            var o = objs[i];
            int cur = prog.currentAmounts[i];
            sb.AppendLine($"{o.description}  <color=#BBBBBB>{cur}/{o.targetAmount}</color>");
        }
        questText.text = sb.ToString();
    }
}
