using NaughtyAttributes;
using UnityEngine;

public class CheatPanel : MonoBehaviour
{
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
}
