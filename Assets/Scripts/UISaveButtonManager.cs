using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UISaveButtonManager : MonoBehaviour
{
    public List<UISaveCreatorLoader> buttons;
    public RectTransform bigPanel;
    public float moveDistance;
    public float panelMoveTime;
    public Ease panelMoveEase;


    private bool _isAnimating;

    public void ShowLoad()
    {
        foreach (UISaveCreatorLoader loader in buttons)
        {
            loader.SetFileCreation(false);
        }
        Move(true);
    }

    public void ShowCreate()
    {foreach (UISaveCreatorLoader loader in buttons)
        {
            loader.SetFileCreation(true);
        }
        Move(true);
    }
    
    public void Move(bool setActive)
    {
        if (_isAnimating) return;
        _isAnimating = true;
        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.Append(DOTween.To(() => bigPanel.offsetMin, x => bigPanel.offsetMin = x,  
            new Vector2(setActive? moveDistance : 0,0) , panelMoveTime)).SetEase(panelMoveEase);
        fadeOutSequence.Join(DOTween.To(() => bigPanel.offsetMax, x => bigPanel.offsetMax = x, 
            new Vector2(setActive? moveDistance : 0,0), panelMoveTime)).SetEase(panelMoveEase);
        fadeOutSequence.OnComplete((() =>  { _isAnimating = false; }));
        //bigPanel.DOMoveX(setActive ? moveDistance : 0, panelMoveTime).SetEase(panelMoveEase).OnComplete((() => _isAnimating = false));
    }
}
