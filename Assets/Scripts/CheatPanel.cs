using NaughtyAttributes;
using UnityEngine;

public class CheatPanel : MonoBehaviour
{
    public MainCharacters mainCharacters;
    [Button]
    public void MaxAngelinaRep()
    {
        GameState.AngelinaReputation += 999;
    }
    [Button]
    public void MaxKarinRep()
    {
        GameState.KarinReputation+= 999;
    }
    [Button]
    public void MaxDariaRep()
    {
        GameState.DariaReputation+= 999;
    }

    [Button]
    public void ResetRep()
    {
        GameState.DariaReputation = 0;
        GameState.KarinReputation = 0;
        GameState.AngelinaReputation = 0;
    }
    [Button]
    public void JeudiDate()
    {
        GameState.CharacterEvent.Add("JEUDI",mainCharacters);
    }

    [Button]
    public void RouteAngelina()
    {
        GameState.CharacterEvent.Add("ROUTE_LOCK_ANGELINA",MainCharacters.Angelina);
    }
    [Button]
    public void RouteKarin()
    {
        GameState.CharacterEvent.Add("ROUTE_LOCK_KARIN",MainCharacters.Karin);
    }
    [Button]
    public void RouteDaria()
    {
        GameState.CharacterEvent.Add("ROUTE_LOCK_DARIA",MainCharacters.Daria);
    }
}
