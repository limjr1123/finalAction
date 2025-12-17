using UnityEngine;

public class PlayerInteractRange : MonoBehaviour
{
    [field:SerializeField] public NPC currentNPC { get; set; } // 현재 상호작용 중인 NPC

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            currentNPC = other.GetComponent<NPC>();
            if (currentNPC != null)
            {
                currentNPC = other.GetComponent<NPC>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            if (currentNPC != null && other.GetComponent<NPC>() == currentNPC)
            {
                currentNPC = null;
            }
        }
    }
}
