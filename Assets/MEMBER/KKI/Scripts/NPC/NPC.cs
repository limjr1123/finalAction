using UnityEngine;


// 나중에 NPC의 속성이 많아지면 상점 NPC, 퀘스트 NPC 등 역할별로 파생 클래스으로 확장
public class NPC : MonoBehaviour, INPCInteractable
{
    [SerializeField] private NPCData npcData;
    [SerializeField] private string startDialogueID;
    private bool isInteract;

    public virtual void Interact()
    {
        if (isInteract == true) return;

        isInteract = true;

        // 상호작용 : 기본 대화, 상점, 퀘스트 수락 등 NPCType에 따라 처리
        // 대화 데이터 보내기
        DialogueManager.Instance.StartDialogue(startDialogueID);
    }

    public virtual void EndInteraction()
    {
        if (isInteract == false) return;

        isInteract = false;


        // UI 및 캐릭터에서 대화가 끝났다는 시그널이 오면 이 함수 실행
    }
}