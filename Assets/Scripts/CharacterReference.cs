using System.Collections.Generic;
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
