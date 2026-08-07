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
    
    
    
    [Header("AutoWaitTimer")]
    [SerializeField] private bool _isOnAuto;
    [SerializeField] private float _autoWaitTime = 0.5f;
    
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
    
    [Header("AutoWaitTimer")]
    [SerializeField] private Animator _dariaAnimator;
    [SerializeField] private Animator _angelinaAnimator;
    [SerializeField] private Animator _karinAnimator;
    [SerializeField] private Animator _cubeChanAnimator;
    
    [Header("Tests")]
    [SerializeField,Expandable] private DialogueSequence _testDialogueSequence;
    
    private float _timeSinceLastDialogue;
    private DialogueSequence _currentDialogueSequence;
    private int _dialogueSequenceIndex;
    private Sequence _sequence;

    private float _autoTimer = -1;
    
    private float _typeWriterTimer;
    private int _typeWriterState; //0 = not writing 1 = writing 2 = writing fast 3 = Force End It
    private int _typeWriterIndex;
    
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
                    if (_dialogueSequenceIndex >= _currentDialogueSequence.dialogues.Count)
                    {
                        EndOfSequence();
                    }
                    else if (_isOnAuto) _autoTimer = 0f;
                }
            }
        }
        else if (_autoTimer >= 0f && _isOnAuto)
        {
            _autoTimer += Time.deltaTime;
            if (_autoTimer >= _autoWaitTime)
            {
                _autoTimer = -1f;
                TryDialogue();
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
        
        if(_typeWriterState == 0 || force)
        {
            if (_dialogueSequenceIndex >= _currentDialogueSequence.dialogues.Count)
            {
                EndOfSequence();
                return;
            }
            
            print("Displaying Dialogue " + _dialogueSequenceIndex + " in " + _currentDialogueSequence.name);
            DisplayDialogue(_currentDialogueSequence.dialogues[_dialogueSequenceIndex]);
            _dialogueSequenceIndex++;
            
        }
        else
        {
            _typeWriterState++;
            print("Type Writer Start: " + _typeWriterState);
            _timeSinceLastDialogue = Time.time;
        }
    }
    
    public void StartDialogue(DialogueSequence dialogueSequence)
    {
        if(!dialogueSequence)
        {
            SetActivation(false);
            return;
        }
        SetActivation(true);
        _currentDialogueSequence = dialogueSequence;
        _dialogueSequenceIndex = 0;
        TryDialogue(true);
    }

    public void DisplayDialogue(Dialogue dialogue, bool skipAnimation = false)
    {
        if (dialogue.changeReputation)
        {
            if (dialogue.isSet) GameState.SetCharacterReputation(dialogue.reputationAmount,dialogue.reputationCharacters);
            else GameState.AddCharacterReputation(dialogue.reputationAmount,dialogue.reputationCharacters);
        }
        
        if (dialogue.triggerAnimation)
        {
            switch (dialogue.animatedCharacters)
            {
                case DatableCharacters.Daria:
                    _dariaAnimator.SetTrigger(dialogue.triggerName);
                    break;
                case DatableCharacters.Angelina:
                    _angelinaAnimator.SetTrigger(dialogue.triggerName);
                    break;
                case DatableCharacters.Karin:
                    _karinAnimator.SetTrigger(dialogue.triggerName);
                    break;
                case DatableCharacters.CubeChan:
                    _cubeChanAnimator.SetTrigger(dialogue.triggerName);
                    break;
            }
        }

        if (dialogue.hasCharacterEvent)
        {
            GameState.CharacterEvent.Add(dialogue.eventName,dialogue.charactersEvent);
        }
        
        dialogue.dialogueEvent?.Invoke();
        
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
        if(_currentDialogueSequence.hasEndChoices)
        {
            int removedChoices = 0;
            for (int i = 0; i < _currentDialogueSequence.choices.Count; i++)
            {
                if (_currentDialogueSequence.choices[i].branching.GetBool())
                    DisplayChoice(_currentDialogueSequence.choices[i], i - removedChoices);
                else removedChoices++;
            }
            return;
        }
        if (_currentDialogueSequence.hasEndBranch)
        {
            StartDialogue(_currentDialogueSequence.GetEndDialogueSequence());
            return;
        }
        SetActivation(false);
    }

    public void DisplayChoice(DialogueChoice choice, int index)
    {
        if (index >= _choicesButtons.Count)
        {
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