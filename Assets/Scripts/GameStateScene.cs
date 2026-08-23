using UnityEngine;

[CreateAssetMenu(menuName = "Create GameStateScene", fileName = "GameStateScene", order = 0)]
public class GameStateScene : ScriptableObject
{
    public int saveIndex = 0;
    public bool isNewGame = false;
}