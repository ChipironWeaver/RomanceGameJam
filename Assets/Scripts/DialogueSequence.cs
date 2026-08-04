using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Dialogue Sequence", fileName = "new DialogueSequence", order = 0)]
public class DialogueSequence : ScriptableObject
{
    [Expandable]public List<Dialogue> dialogues;
}