using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static float DariaReputation{ get; private set; }
    public static float AngelinaReputation{ get; private set; }
    public static float KarinReputation{ get; private set; }
    public static float CubeChanReputation{ get; private set; }
    
    public static Dictionary<string, DatableCharacters> CharacterEvent;

    public static void SetCharacterReputation(float reputation,DatableCharacters character)
    {
        switch (character)
        {
            case DatableCharacters.Daria:
                DariaReputation = reputation;
                break;
            case DatableCharacters.Angelina:
                AngelinaReputation = reputation;
                break;
            case DatableCharacters.Karin:
                KarinReputation = reputation;
                break;
            case DatableCharacters.CubeChan:
                CubeChanReputation = reputation;
                break;
        }
    }
    
    public static void AddCharacterReputation(float reputation,DatableCharacters character)
    {
        switch (character)
        {
            case DatableCharacters.Daria:
                DariaReputation += reputation;
                break;
            case DatableCharacters.Angelina:
                AngelinaReputation += reputation;
                break;
            case DatableCharacters.Karin:
                KarinReputation += reputation;
                break;
            case DatableCharacters.CubeChan:
                CubeChanReputation += reputation;
                break;
        }
    }

    public static float GetCharacterReputation(DatableCharacters character)
    {
        switch (character)
        {
            case DatableCharacters.Daria:
                return DariaReputation;
            case DatableCharacters.Angelina:
                return AngelinaReputation;
            case DatableCharacters.Karin:
                return KarinReputation;
            case DatableCharacters.CubeChan:
                return CubeChanReputation;
        }
        return 0;
    }

    void OnEnable()
    {
        Singleton();
    }
    public static GameState Instance{ get; private set; }
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

public enum DatableCharacters
{
    None,
    Daria,
    Angelina,
    Karin,
    CubeChan
}
