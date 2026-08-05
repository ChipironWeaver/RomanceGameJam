using UnityEngine;

public class GameState : MonoBehaviour
{
    public static float DariaReputation{ get; private set; }
    public static float AngelinaReputation{ get; private set; }
    public static float KarinReputation{ get; private set; }

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
        }
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
    Daria,
    Angelina,
    Karin
}
