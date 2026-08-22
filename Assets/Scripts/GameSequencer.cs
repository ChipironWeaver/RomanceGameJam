using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class GameSequencer : MonoBehaviour
{
    [SerializeField] private List<GameStateAction> _actions;
    [SerializeField] private bool _autostart;
    [Header("References")]
    [SerializeField] private BlackScreen _blackScreen;
    [SerializeField] private DialogueController _dialogueController;
    [SerializeField] private BarTendingController _barTendingController;
    [SerializeField] private CharacterReference _characterReference;

    public static int CurrentIndex = -1;
    public static int LatestScore;

    //Receive the actions, check the current state + continue on the index

    private void OnEnable()
    {
        Actions.EndOfBlackScreenPhase += () => { ActionReceiver(TypeOfState.BlackScreen); };
        Actions.EndOfVisualNovelPhase += () => { ActionReceiver(TypeOfState.VisualNovel); };
        Actions.EndOfGameplayPhase += () => { ActionReceiver(TypeOfState.BarTending); };
        Actions.EndOtherActionPhase += () => { ActionReceiver(TypeOfState.Other); };
        Actions.CharacterMoved += () => { ActionReceiver(TypeOfState.CharacterMoved); };
    }
    private void ActionReceiver(TypeOfState state)
    {
        if(state == TypeOfState.BarTending && _barTendingController.latestScore != 0) LatestScore = _barTendingController.latestScore; 
        if (CurrentIndex == -1) return;
        if (CurrentIndex > _actions.Count) return; 
        if(state == _actions[CurrentIndex-1].state) NextAction();
    }

    public void Start()
    {
        if(_autostart) StartGame();
    }

    public void StartGame(int index = 0)
    {
        CurrentIndex = index;
        NextAction();
    }

    public void NextAction()
    {
        if (CurrentIndex >= _actions.Count)
        {
            print("End Of Game");
            return;
        }
        GameStateAction action = _actions[CurrentIndex];
        switch(action.state)
        {
            case TypeOfState.BlackScreen:
                _blackScreen.ShowBlackScreen(action.bigText, action.smallText);
                break;
            case TypeOfState.VisualNovel:
                if (CurrentIndex - 1 >= 0)
                {
                    if (_actions[CurrentIndex - 1].state == TypeOfState.BarTending && action.changeBasedOnLastResult)
                    {
                        switch (LatestScore)
                        {
                            case 1:
                                _dialogueController.StartDialogue(action.badSequence);
                                return;
                            case 2:
                                _dialogueController.StartDialogue(action.averageSequence);
                                return;
                            case 3:
                                _dialogueController.StartDialogue(action.goodSequence);
                                return;
                        }
                    }
                }
                _dialogueController.StartDialogue(action.sequence);
                break;
            case TypeOfState.BarTending:
                _barTendingController.GameplayStart(action.day, action.mainCharacters,false,action.isSpecial ? action.datableRecipe : null);
                break;
            case TypeOfState.CharacterMoved:
                _characterReference.SetActivation(action.mainCharacters, action.activation,action.instantMove);
                break;
            case TypeOfState.Other:
                action.otherEvent.Invoke();
                break;
        }
        CurrentIndex++;
    }
[Button]
    public void Test()
    {
        StartGame(0);
    }
    
    [Serializable]
    private class GameStateAction
    {
        public string name;
        
        public TypeOfState state;

        [Header("BlackScreen")] 
        public String bigText;
        public String smallText;
        
        [Header("BarTending")]
        public float day;
        public MainCharacters mainCharacters;
        public bool isSpecial;
        public Recipe datableRecipe;
        
        [Header("Visual Novel")]
        public DialogueSequence sequence;

        public bool changeBasedOnLastResult;
        public DialogueSequence goodSequence;
        public DialogueSequence averageSequence;
        public DialogueSequence badSequence;

        [Header("Character Moved")]
        public MainCharacters moveMainCharacters;

        public bool activation;
        public bool instantMove;
        
        [Header("Other")]
        public UnityEvent otherEvent;
        
    }
    private enum TypeOfState
    {
        BlackScreen,
        BarTending,
        VisualNovel,
        ImageShown,
        CharacterMoved,
        Other,
    }
}
