using System;
using System.Collections;
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
    [Header("TypeWriterSettings")]
    [SerializeField] private float _speedMultiplier;
    [SerializeField] private float _skippingSpeedMultiplier;
    [SerializeField] private float _timeToWaitPerCharacter;
    [SerializeField] private float _timeToWaitForSpaces;
    [SerializeField] private List<string> _hiddenCharacters;
    [SerializeField] private List<TypeWriterCharacterWait> _specialCharacterWaits = new List<TypeWriterCharacterWait>();
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
    
    private int _typeWriterState; //0 = not writing 1 = writing 2 = writing fast 3 = Force End It
    private int _typeWriterIndex;
    private float _typeWriterTimer;
    List<float> _typeWriterWaitTimes = new List<float>();
    
    
    private List<ChoiceButton> _choicesButtons = new List<ChoiceButton>();
    private Dictionary<char, float> _specialCharacterWaitsDictionary;

    public void Update()
    {
        if (_typeWriterState > 0)
        {
            if (_typeWriterState == 3)
            {
                _textMeshProUGUI.maxVisibleCharacters = _typeWriterWaitTimes.Count;
                _typeWriterState = 0;
                return;
            }
            _typeWriterTimer += Time.deltaTime;
            if (_typeWriterTimer >= _typeWriterWaitTimes[_typeWriterIndex] /
                     (_speedMultiplier * (_typeWriterState > 1 ? _skippingSpeedMultiplier : 1)))
            {
                _typeWriterIndex++;
                _typeWriterTimer = 0;
                _textMeshProUGUI.maxVisibleCharacters++;
                if (_typeWriterIndex >= _typeWriterWaitTimes.Count)
                {
                    _textMeshProUGUI.maxVisibleCharacters = _typeWriterWaitTimes.Count;
                    _typeWriterState = 0;
                }
            }
        }
    }
    
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

    public void DisplayDialogue(Dialogue dialogue, bool skipAnimation = false)
    {
        if (_specialCharacterWaitsDictionary == null)
        {
            _specialCharacterWaitsDictionary = new Dictionary<char, float>();
            foreach (TypeWriterCharacterWait wait in _specialCharacterWaits)
            {
                _specialCharacterWaitsDictionary.Add(wait.character.ToCharArray()[0], wait.waitTime);
            }
        }
        
        string textFinalString = "";
        _typeWriterWaitTimes = new List<float>();
        bool isInMarkdown = false;
        bool previousCharacterHiddenSpecialTime = false;
        bool ignoreSpecialBehavior = false;
        
        foreach(char character in dialogue.dialogueText)
        {
            if (ignoreSpecialBehavior)
            {
                textFinalString += character;
                _typeWriterWaitTimes.Add(_timeToWaitPerCharacter);
                ignoreSpecialBehavior = false;
            }
            else if (isInMarkdown)
            {
                textFinalString += character;
                if (character == ">".ToCharArray()[0])
                {
                    isInMarkdown = false;
                }
            }
            else if (character == "$".ToCharArray()[0])
            {
                ignoreSpecialBehavior = true;
            }
            else if (character == "<".ToCharArray()[0])
            {
                textFinalString += character;
                isInMarkdown = true;
            }
            else if (character == " ".ToCharArray()[0])
            {
                textFinalString += character;
                _typeWriterWaitTimes.Add(_timeToWaitForSpaces);
            }
            else if (_specialCharacterWaitsDictionary.ContainsKey(character) && !previousCharacterHiddenSpecialTime)
            {
                _typeWriterWaitTimes.Add(_specialCharacterWaitsDictionary[character]);
                if (!_hiddenCharacters.Contains(character.ToString()))
                {
                    textFinalString += character;
                }
                else
                {
                    previousCharacterHiddenSpecialTime = true;
                }
            }
            else
            {
                textFinalString += character;
                if(!previousCharacterHiddenSpecialTime)
                {
                    _typeWriterWaitTimes.Add(_timeToWaitPerCharacter);
                }
                else
                {
                    previousCharacterHiddenSpecialTime = false;
                }
            }
        }
        
        _textMeshProUGUI.text = textFinalString;
        if(skipAnimation)
        {
            _textMeshProUGUI.maxVisibleCharacters = _typeWriterWaitTimes.Count;
        }
        else
        {
            print("time slot :" + _typeWriterWaitTimes.Count);
            _textMeshProUGUI.maxVisibleCharacters = 0;
            _typeWriterState = 1;
            _typeWriterIndex = 0;
            _typeWriterTimer = 0f;
        }
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
    public void SetActivation(bool active)
    {
        _dialoguePanel.SetActive(active);
    }
}

[Serializable]
public class TypeWriterCharacterWait
{
    public string character;
    public float waitTime;
}