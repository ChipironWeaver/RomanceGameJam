using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private float _dialogueSkipCooldown;
    [SerializeField] private GameObject _choiceButtonPrefab;
    [Header("References")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private GameObject _choicePanel;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [Header("Tests")]
    [SerializeField,Expandable] private DialogueSequence _testDialogueSequence;
    
    private float _timeSinceLastDialogue;
    private DialogueSequence _currentDialogueSequence;
    private int _dialogueSequenceIndex;
    private Sequence _sequence;
    private List<ChoiceButton> _choicesButtons = new List<ChoiceButton>();

    public void TryDialogue(bool force = false)
    {
        if (!(Time.time > _timeSinceLastDialogue + _dialogueSkipCooldown) && !force)
        {
            print("Can't");
            return;
        }
        
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
        print("End of Sequence");
        for (int i = 0; i < _currentDialogueSequence.choices.Count; i++)
        {
            print( "Displaying Button : " + _currentDialogueSequence.choices[i].choiceString + " n°" +  i);
            DisplayChoice(_currentDialogueSequence.choices[i],i);
        }
    }

    public void DisplayChoice(DialogueChoice choice, int index)
    {
        if (index >= _choicesButtons.Count)
        {
            print("New button");
            GameObject newButton = Instantiate(_choiceButtonPrefab, _choicePanel.transform);
            newButton.name = "ChoiceButton " + (index + 1);
            _choicesButtons.Add(newButton.GetComponent<ChoiceButton>());
        }
        _choicesButtons[index].gameObject.SetActive(true);
        _choicesButtons[index].Initialize(choice,this);
    }

    public void HideChoices()
    {
        foreach (ChoiceButton button in _choicesButtons)
        {
            if (button.isActiveAndEnabled)
            {
                button.FadeOut();
            }
        }
    }
    
    [Button]
    public void TestDialogueSequence()
    {
        StartDialogue(_testDialogueSequence);
    }
}