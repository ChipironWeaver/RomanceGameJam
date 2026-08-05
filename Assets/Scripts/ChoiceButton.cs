using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _choiceButtonText;
    [SerializeField] private Button _choiceButton;


    public void Initialize(DialogueChoice choice, DialogueController linkedController)
    {
        _choiceButtonText.text = choice.choiceString;
        _choiceButton.onClick.RemoveAllListeners();
        _choiceButton.onClick.AddListener(() =>
        {
            choice.DialogueClicked();
            linkedController.HideChoices();
            if(choice.linkedDialogueSequence) linkedController.StartDialogue(choice.linkedDialogueSequence);
            else linkedController.SetActivation(false);
            
        });
    }
    
    public void FadeOut()
    {
        _choiceButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        
    }
}
