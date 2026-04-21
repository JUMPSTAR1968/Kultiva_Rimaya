using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    [Header("Conversation Data")]
    // This exposes your DialogueLine class to the Inspector
    [SerializeField] private DialogueLine[] conversation;

    // Call this method via a UI Button, a 2D trigger collider, or Raycast
    public void Interact()
    {
        // Pass the authored array to the Singleton manager
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(conversation);
        }
        else
        {
            Debug.LogError("No DialogueManager found in the scene!");
        }
    }
}