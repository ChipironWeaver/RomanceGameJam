using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Create Dialogue", fileName = "new Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    [ResizableTextArea]
    public string dialogueText;

    public bool triggerAnimation;
    
    [ShowIf("triggerAnimation")] public DatableCharacters characters;
    [ShowIf("triggerAnimation")] public string triggerName;

    public bool invokeEvents;

    [ShowIf("invokeEvents")] public UnityEvent dialogueStartEvent;
    [ShowIf("invokeEvents")] public UnityEvent dialogueEndEvent;

    public bool changeReputation;
    [ShowIf("changeReputation")] public bool isSet;
    [ShowIf("changeReputation")] public float reputationAmount;

}