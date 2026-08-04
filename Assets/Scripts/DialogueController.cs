using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private float _dialogueSkipCooldown;
    
    private float _timeSinceLastDialogue;

    public void TryDialogue()
    {
        if (Time.time > _timeSinceLastDialogue + _dialogueSkipCooldown)
        {
            print("Yes");
        }
        else print("nuh uh");
    }
    
    public void StartDialogue(DialogueSequence dialogueSequence)
    {
        
    }

    public float EstimateDialogueSequenceTime(DialogueSequence dialogueSequence)
    {
        return 67f;
    }

    public float EstimateDialogueTime(Dialogue dialogue)
    {
        return 6.7f;
    }
}