using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private float _dialogueSkipCooldown;
    
    [Header("References")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [Header("Tests")]
    [SerializeField] private DialogueSequence _testDialogueSequence;
    
    private float _timeSinceLastDialogue;
    private DialogueSequence _currentDialogueSequence;
    private int _dialogueSequenceIndex;
    private Sequence _sequence;

    public void TryDialogue(bool force = false)
    {
        if (!(Time.time > _timeSinceLastDialogue + _dialogueSkipCooldown) && !force) return;
        
        if (_dialogueSequenceIndex >= _currentDialogueSequence.dialogues.Count)
        {
            EndOfSequence();
            return;
        }
        
        print("Displaying Dialogue " + _dialogueSequenceIndex + " in " + _currentDialogueSequence.name);
        DisplayDialogue(_currentDialogueSequence.dialogues[_dialogueSequenceIndex]);
        
        _dialogueSequenceIndex++;
    }
    
    public void StartDialogue(DialogueSequence dialogueSequence)
    {
        SetActivation(true);
        _currentDialogueSequence = dialogueSequence;
        _dialogueSequenceIndex = 0;
        TryDialogue(true);
    }

    public void SetActivation(bool active)
    {
        _dialoguePanel.SetActive(active);
    }

    public void DisplayDialogue(Dialogue dialogue)
    {
        _textMeshProUGUI.text = dialogue.dialogueText;
        _textMeshProUGUI.maxVisibleCharacters = 67;
    }

    public void EndOfSequence()
    {
        print("this is the end of the sequence");
        SetActivation(false);
    }

    
    [Button]
    public void TestDialogueSequence()
    {
        StartDialogue(_testDialogueSequence);
    }
}