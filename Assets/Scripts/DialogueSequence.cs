using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Create Dialogue Sequence", fileName = "new DialogueSequence", order = 0)]
public class DialogueSequence : ScriptableObject
{
    [Expandable] public List<Dialogue> dialogues;
    
    [Header("End Branch Options")]
    public bool hasEndChoices;
    [ShowIf("hasEndChoices")] public List<DialogueChoice> choices;
    
    public bool hasEndBranch;
    [ShowIf("hasEndBranch")] public List<EndBranch> endBranches;

    public DialogueSequence GetEndDialogueSequence()
    {
        foreach (EndBranch endBranch in endBranches)
        {
            if (endBranch.branching.GetBool())  return endBranch.linkedDialogueSequence;
        }
        return null;
    }
}

public class DialogueChoice
{
    public string choiceString;
    public DialogueSequence linkedDialogueSequence;
    public UnityEvent onDialogueChoice;
    public DialogueBranching branching;
    
    public void DialogueClicked()
    {
        onDialogueChoice?.Invoke();
    }
}

public class EndBranch
{
    public DialogueBranching branching;
    public DialogueSequence linkedDialogueSequence;
}