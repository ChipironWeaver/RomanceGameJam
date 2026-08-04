using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Create Dialogue", fileName = "new Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    [ResizableTextArea]
    public string dialogueText;
    public UnityEvent dialogueStartEvent;
    public UnityEvent dialogueEndEvent;
    
}