using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Create Dialogue", fileName = "new Dialogue", order = 0)]
public class Dialogue : ScriptableObject
{
    [ResizableTextArea]
    public string dialogueText;

    public bool setBlackScreen;
    [ShowIf("setBlackScreen")] public bool blackScreen;
    [ShowIf("setBlackScreen")] public bool setImage;
    [ShowIf("setBlackScreen")] public Sprite imageSprite;
    
    [Header("NameDisplay")]
    public MainCharacters speakingCharacter;
    public string npcName;
    public Sprite npcIcon;

    [Header("Audio")] 
    public bool playMusic;
    [ShowIf("playMusic")]public int musicIndex;
    public bool playSFX;
    [ShowIf("playSFX")]public SfxType sfxType;

    
    [Header("Animation")]
    public bool triggerAnimation;
    [ShowIf("triggerAnimation")] public MainCharacters animatedCharacters;
    [ShowIf("triggerAnimation")] public string triggerName;
    
    [Header("Show or Hide Character")]
    public bool showCharacter;
    public bool hideCharacter;
    [ShowIf("showCharacter")] public MainCharacters shownCharacters;
    [ShowIf("hideCharacter")] public MainCharacters hiddenCharacters;
    
    [Header("Reputation")]
    public bool changeReputation;
    [ShowIf("changeReputation")] public MainCharacters reputationCharacters;
    [ShowIf("changeReputation")] public bool isSet;
    [ShowIf("changeReputation")] public float reputationAmount;

    public bool hasCharacterEvent;
    [ShowIf("hasCharacterEvent")] public MainCharacters charactersEvent;
    [ShowIf("hasCharacterEvent")] public string eventName;
    
    public UnityEvent dialogueEvent;
}