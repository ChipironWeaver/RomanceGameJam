using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static string PlayerName;
    public string testPlayerName;
    
    public static float DariaReputation{ get; private set; }
    public static float AngelinaReputation{ get; private set; }
    public static float KarinReputation{ get; private set; }
    public static float CubeChanReputation{ get; private set; }
    
    public static Dictionary<string, MainCharacters> CharacterEvent = new Dictionary<string, MainCharacters>();

    public static void SetCharacterReputation(float reputation,MainCharacters character)
    {
        switch (character)
        {
            case MainCharacters.Daria:
                DariaReputation = reputation;
                break;
            case MainCharacters.Angelina:
                AngelinaReputation = reputation;
                break;
            case MainCharacters.Karin:
                KarinReputation = reputation;
                break;
            case MainCharacters.CubeChan:
                CubeChanReputation = reputation;
                break;
        }
    }
    
    public static void AddCharacterReputation(float reputation,MainCharacters character)
    {
        switch (character)
        {
            case MainCharacters.Daria:
                DariaReputation += reputation;
                break;
            case MainCharacters.Angelina:
                AngelinaReputation += reputation;
                break;
            case MainCharacters.Karin:
                KarinReputation += reputation;
                break;
            case MainCharacters.CubeChan:
                CubeChanReputation += reputation;
                break;
        }
    }

    public static float GetCharacterReputation(MainCharacters character)
    {
        switch (character)
        {
            case MainCharacters.Daria:
                return DariaReputation;
            case MainCharacters.Angelina:
                return AngelinaReputation;
            case MainCharacters.Karin:
                return KarinReputation;
            case MainCharacters.CubeChan:
                return CubeChanReputation;
        }
        return 0;
    }

    void OnEnable()
    {
        Singleton();
        if (PlayerName == null && testPlayerName != null)
        {
            PlayerName = testPlayerName;
        }
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

public enum MainCharacters
{
    None,
    Player,
    Daria,
    Angelina,
    Karin,
    CubeChan
}