using UnityEngine;


// 플레이어 스크립트에 이 코드 넣어주기
public class NPCTest : MonoBehaviour
{
    public GameObject TestNPC;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Test();
        }
    }

    void Test()
    {
        var npc = TestNPC.GetComponent<NPC>();

        if (npc == null)
        {
            Debug.LogWarning("NPC 없음");
            return;
        }

        if (npc is INPCInteractable)
        {
            npc.Interact();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        var npc = other.GetComponent<NPC>();
        if (npc is INPCInteractable)
        {
            // 상호작용 키를 누르면
            npc.Interact();
        }
    }
}