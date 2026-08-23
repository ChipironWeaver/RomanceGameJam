using System;
using UnityEngine;

public class GameStateLoader : MonoBehaviour
{
    public GameStateScene gameStateScene;
    public void Start()
    {
        if (gameStateScene)
        {
            if (gameStateScene.saveIndex > 0)
            {
                print("loading save " + gameStateScene.saveIndex);
                gameStateScene.saveIndex = 0;
            }
        }
    }
}