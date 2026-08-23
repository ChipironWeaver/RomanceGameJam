using UnityEngine;

public class UISaveCreatorLoader : MonoBehaviour
{
    public int currentIndex;
    public GameStateScene stateScene;
    public bool inFileCreation;
    
    
    public void SetAsActiveSave()
    {
        if(currentIndex > 0) 
        {
            stateScene.saveIndex = currentIndex;
            stateScene.isNewGame = false;//CheckIfFileExist
        }
    }
}
