using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterReference : MonoBehaviour
{
    [Header("Angelina")]
    public Animator angelinaAnimator;
    public GameObject angelinaObject;
    [Header("Daria")]
    public Animator dariaAnimator;
    public GameObject dariaObject;
    [Header("Karin")]
    public Animator karinAnimator;
    public GameObject karinObject;
    [Header("Npc")] public List<GameObject> npcs;
    
    [Header("Postions")]
    [SerializeField] private Vector3 _activePosition;
    [SerializeField] private Vector3 _notActivePosition;
    [SerializeField] private float _moveTime;
    [SerializeField] private Ease _easeType;
    
    private GameObject _currentNpcObject;

    public void Start()
    {
        angelinaObject.SetActive(false);
        dariaObject.SetActive(false);
        karinObject.SetActive(false);
        foreach (GameObject npc in npcs) npc.SetActive(false);
    }
    public void SetActivation(MainCharacters characters,bool active,bool instant = false)
    {
        print("trying to move : " + GetGameObject(characters).name + active);
        GameObject character = null;
        if (characters == MainCharacters.None)
        {
            if (!active && !_currentNpcObject)
            {
                print("Deactivated on deactivation Return");
                return;
            }
            if (active && _currentNpcObject)
            {
                print(active);
                print("activated on activation Return");
                return;
            }
            if(active)
            {
                _currentNpcObject = npcs[Random.Range(0, npcs.Count)];
            }
            character = _currentNpcObject;
        }
        else
        {
            character = GetGameObject(characters);
            if (character.activeSelf == active)
            {
                print("Character is active");
                return;
            }
        }
        
        if (!character)
        {
            print("character is null");
            return;
        }
        if(active)character.SetActive(true);
        if (instant)
        {
            print(character.name + " has been instant moved");
            character.transform.position = active ? _activePosition : _notActivePosition;
            if(!active) character.SetActive(false);
            Actions.CharacterMoved?.Invoke();
        }
        else character.transform.DOMove(active ? _activePosition : _notActivePosition, 0.5f).SetEase(_easeType).OnComplete(()=>
        {
            print(character.name + " has been moved");
            Actions.CharacterMoved?.Invoke();
            if(!active)
            {
                character.SetActive(false);
                if (characters == MainCharacters.None) _currentNpcObject = null;
            }
        });
        
    }
    
    public GameObject GetGameObject(MainCharacters characters)
    {
        switch (characters)
        {
            case(MainCharacters.Angelina):
                return angelinaObject;
            case(MainCharacters.Daria):
                return dariaObject;
            case(MainCharacters.Karin):
                return karinObject;
            case(MainCharacters.None):
                return npcs[Random.Range(0, npcs.Count)];
        }

        return null;
    }
    
    public Animator GetAnimatorObject(MainCharacters characters)
    {
        switch (characters)
        {
            case(MainCharacters.Angelina):
                return angelinaAnimator;
            case(MainCharacters.Daria):
                return dariaAnimator;
            case(MainCharacters.Karin):
                return karinAnimator;
        }

        return null;
    }
    
    
    public void OnEnable()
    {
        Singleton();
    }
    public static CharacterReference Instance { get; private set; }
    void Singleton()
    {
        if (Instance !=null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
