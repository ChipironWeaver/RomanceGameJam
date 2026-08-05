using System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueChoice
{
    public string choiceString;
    public DialogueSequence linkedDialogueSequence;
    public UnityEvent onDialogueChoice;
    public DialogueChoiceConditions choiceConditions;
    public DatableCharacters charactersReputation;
    public float reputationAmount;
    
    public void DialogueClicked()
    {
        onDialogueChoice?.Invoke();
        if (choiceConditions.HasFlag(DialogueChoiceConditions.Reputation) ^ choiceConditions.HasFlag(DialogueChoiceConditions.InvertCondition))
        {
            GameState.AddCharacterReputation(reputationAmount, charactersReputation);
        }
    }
}
