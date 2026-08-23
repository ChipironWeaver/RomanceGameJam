using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISaveCreatorLoader : MonoBehaviour
{
    public int currentIndex;
    public GameStateScene stateScene;
    public bool inFileCreation;
    public bool saveExists;
    public TextMeshProUGUI bigText;
    public TextMeshProUGUI smallText;
    public Button saveButton;
    public int day;

    public void Start()
    {
        if(currentIndex > 0)
        {
            GameStateLoader.GameSaveClass saveClass = GameStateLoader.Instance.LoadSave(currentIndex);
            if (saveClass != null)
            {
                saveExists = true;
                day = saveClass.currentDay;
            }
        }
    }

    public void SetFileCreation(bool isNewGame)
    {
        inFileCreation = isNewGame;
        smallText.text = saveExists ? "Jour " + day : "Vide";
        if (inFileCreation)
        {
            saveButton.interactable = true;
            bigText.text = "<font-weight=\"600\"> " + (saveExists ? "Supprimer et créer" : "Créer") ;
        }
        else
        {
            saveButton.interactable = saveExists;
            bigText.text = "<font-weight=\"600\"> " + (saveExists ? "Charger" : "Sauvegarde Vide") ;
        }
    }
    
    public void SetAsActiveSave()
    {
        if(currentIndex > 0) 
        {
            stateScene.saveIndex = currentIndex;
            stateScene.isNewGame = !saveExists || inFileCreation ;
        }
    }
}
