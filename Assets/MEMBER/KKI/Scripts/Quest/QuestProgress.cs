public class QuestProgress
{
    public QuestData questData;
    public int[] currentAmounts;
    public bool isCompleted;

    public QuestProgress(QuestData data)
    {
        questData = data;
        currentAmounts = new int[data.questObjectives.Length];
        isCompleted = false;
    }
}