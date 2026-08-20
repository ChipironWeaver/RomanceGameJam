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
    [SerializeField] private UIAnimator.AnimationObject _panelAnimation;
    [Header("PlayerAndCharacterIcons")] 
    [SerializeField] private Sprite _playerIcon;
    [SerializeField] private Sprite _dariaIcon;
    [SerializeField] private Sprite _angelinaIcon;
    [SerializeField] private Sprite _karinIcon;
    [SerializeField] private Sprite _cubeChanIcon;

    [Header("AutoWaitTimer")] [SerializeField]
    private bool _isOnAuto;

    [SerializeField] private float _autoWaitTime = 0.5f;

    [Header("TypeWriterSettings")] [SerializeField]
    private float _speedMultiplier;

    [SerializeField] private float _skippingSpeedMultiplier;
    [SerializeField] private float _timeToWaitPerCharacter;
    [SerializeField] private float _timeToWaitForSpaces;
    [SerializeField] private List<string> _hiddenCharacters;
    [SerializeField] private List<TypeWriterCharacterWait> _specialCharacterWaits = new List<TypeWriterCharacterWait>();

    [Header("References")] [SerializeField]
    private GameObject _dialoguePanel;

    [SerializeField] private Image _nextButton;
    [SerializeField] private GameObject _choicePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private Image _characterIcon;
    
    [Header("Tests")] [SerializeField, Expandable]
    private DialogueSequence _testDialogueSequence;

    private float _timeSinceLastDialogue;
    private DialogueSequence _currentDialogueSequence;
    private int _dialogueSequenceIndex;
    private Sequence _sequence;

    private float _autoTimer = -1;

    private float _typeWriterTimer;
    private int _typeWriterState; //0 = not writing 1 = writing 2 = writing fast 3 = Force End It
    private int _typeWriterIndex;
    private bool _endBranchShown;

    List<float> _typeWriterWaitTimes = new List<float>();

    private List<ChoiceButton> _choicesButtons = new List<ChoiceButton>();
    private Dictionary<char, float> _specialCharacterWaitsDictionary;

    public void Update()
    {
        if (_typeWriterState > 0)
        {
            if (_typeWriterState == 3)
            {
                _dialogueText.maxVisibleCharacters = _typeWriterWaitTimes.Count;
                _typeWriterState = 0;
                return;
            }

            _typeWriterTimer += Time.deltaTime;
            if (_typeWriterTimer >= _typeWriterWaitTimes[_typeWriterIndex] /
                (_speedMultiplier * (_typeWriterState > 1 ? _skippingSpeedMultiplier : 1)))
            {
                _typeWriterIndex++;
                _typeWriterTimer = 0;
                _dialogueText.maxVisibleCharacters++;
                if (_typeWriterIndex >= _typeWriterWaitTimes.Count)
                {
                    _dialogueText.maxVisibleCharacters = _typeWriterWaitTimes.Count;
                    _typeWriterState = 0;
                    if (_dialogueSequenceIndex >= _currentDialogueSequence.dialogues.Count)
                    {
                        EndOfSequence();
                        if(_endBranchShown && _isOnAuto) _autoTimer = 0f;
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

        if (_typeWriterState == 0 || force)
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
        if (!dialogueSequence)
        {
            SetActivation(false);
            return;
        }

        SetActivation(true);
        _currentDialogueSequence = dialogueSequence;
        _dialogueSequenceIndex = 0;
        TryDialogue(true);
    }

    private void DisplayDialogue(Dialogue dialogue, bool skipAnimation = false, bool isSingular = false)
    {
        if (dialogue.changeReputation)
        {
            if (dialogue.isSet)
                GameState.SetCharacterReputation(dialogue.reputationAmount, dialogue.reputationCharacters);
            else GameState.AddCharacterReputation(dialogue.reputationAmount, dialogue.reputationCharacters);
        }

        if (dialogue.triggerAnimation)
        {
            Animator animator = CharacterReference.Instance.GetAnimatorObject(dialogue.animatedCharacters);
            if(animator) animator.SetTrigger(dialogue.triggerName);
        }

        if (dialogue.hasCharacterEvent)
        {
            GameState.CharacterEvent.Add(dialogue.eventName, dialogue.charactersEvent);
        }

        Sprite icon = null;

        switch (dialogue.speakingCharacter)
        {
            case MainCharacters.None:
                icon = dialogue.npcIcon;
                _characterNameText.text = dialogue.npcName;
                break;
            case MainCharacters.Player:
                icon = _playerIcon;
                _characterNameText.text = GameState.PlayerName;
                break;
            case MainCharacters.Karin:
                icon = _karinIcon;
                _characterNameText.text = "Karin";
                break;
            case MainCharacters.Daria:
                icon = _dariaIcon;
                _characterNameText.text = "Daria";
                break;
            case MainCharacters.Angelina:
                icon = _angelinaIcon;
                _characterNameText.text = "Angelina";
                break;
            case MainCharacters.CubeChan:
                icon = _cubeChanIcon;
                _characterNameText.text = "Cube Chan";
                break;
        }
        
        if (icon)
        {
            _characterIcon.sprite = icon;
            _characterIcon.color = Color.white;
        }
        else
        {
            _characterIcon.color = Color.clear;
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

        foreach (char character in dialogue.dialogueText)
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
                if (!previousCharacterHiddenSpecialTime)
                {
                    _typeWriterWaitTimes.Add(_timeToWaitPerCharacter);
                }
                else
                {
                    previousCharacterHiddenSpecialTime = false;
                }
            }
        }

        _dialogueText.text = textFinalString;
        if (skipAnimation)
        {
            _dialogueText.maxVisibleCharacters = _typeWriterWaitTimes.Count;
        }
        else
        {
            print("time slot :" + _typeWriterWaitTimes.Count);
            _dialogueText.maxVisibleCharacters = 0;
            _typeWriterState = 1;
            _typeWriterIndex = 0;
            _typeWriterTimer = 0f;
        }
    }

    public void EndOfSequence()
    {
        print("End of Sequence");
        if (_currentDialogueSequence.hasEndChoices)
        {
            _currentDialogueSequence.endEvent?.Invoke();
            int removedChoices = 0;
            for (int i = 0; i < _currentDialogueSequence.choices.Count; i++)
            {
                if (_currentDialogueSequence.choices[i].branching.GetBool())
                    DisplayChoice(_currentDialogueSequence.choices[i], i - removedChoices);
                else removedChoices++;
            }

            return;
        }

        if (_endBranchShown)
        {
            _endBranchShown = false;
            _currentDialogueSequence.endEvent?.Invoke();
            if (_currentDialogueSequence.hasEndBranch) StartDialogue(_currentDialogueSequence.GetEndDialogueSequence());
            else SetActivation(false);
        }
        else _endBranchShown = true;
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
        _choicesButtons[index].Initialize(choice, this);
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
        if (_dialoguePanel.gameObject.activeSelf == active) return;
        print("<color = #FF0000>Set Activation : " + active);
        _panelAnimation.Animate(!active);
        _nextButton.raycastTarget = active;
    }
}

[Serializable]
public class TypeWriterCharacterWait
{
    public string character;
    public float waitTime;
}