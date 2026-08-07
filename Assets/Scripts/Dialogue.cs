using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Create Dialogue", fileName = "new Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    [ResizableTextArea]
    public string dialogueText;

    [Header("Animation")]
    public bool triggerAnimation;
    
    [ShowIf("triggerAnimation")] public DatableCharacters animatedCharacters;
    [ShowIf("triggerAnimation")] public string triggerName;
    
    [Header("Reputation")]
    public bool changeReputation;
    [ShowIf("changeReputation")] public DatableCharacters reputationCharacters;
    [ShowIf("changeReputation")] public bool isSet;
    [ShowIf("changeReputation")] public float reputationAmount;

    public bool hasCharacterEvent;
    [ShowIf("hasCharacterEvent")] public DatableCharacters charactersEvent;
    [ShowIf("hasCharacterEvent")] public string eventName;
    
    public UnityEvent dialogueEvent;
}