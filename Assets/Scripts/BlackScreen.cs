using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _bigText;
    [SerializeField] private TextMeshProUGUI _smallText;
    [SerializeField] private string _bigTextTag;
    [SerializeField] private string _smallTextTag;
    [SerializeField] private Image _image;
    
    [Header("Setting")]
    [SerializeField] private Color _textColor;
    [SerializeField] private Color _backgroundColor;
    [SerializeField] private float _animationTime;

    private Sequence _sequence;
    

    [Button]
    public void Test()
    {
        ShowBlackScreen("Jour 1", "16h29");
    }
    
    public void ShowBlackScreen(string bigText, string smallText)
    {
        Color bgColor = new Color(_backgroundColor.r, _backgroundColor.g, _backgroundColor.b, 0);
        Color textColor = new Color(_textColor.r, _textColor.g, _textColor.b, 0);
        
        _image.color = bgColor;
        _bigText.color = textColor;
        _smallText.color = textColor;
        
        _sequence = DOTween.Sequence();
        _sequence.Append(_image.DOColor(_backgroundColor, _animationTime/2));
        _sequence.Append(_bigText.DOColor(_textColor, _animationTime/2));
        _sequence.Join(_smallText.DOColor(_textColor, _animationTime/2));
        _sequence.AppendInterval(_animationTime);
        _sequence.Append(_bigText.DOColor(textColor, _animationTime/2));
        _sequence.Join(_smallText.DOColor(textColor, _animationTime/2));
        _sequence.Append(_image.DOColor(bgColor, _animationTime/2));
        _sequence.AppendCallback(() => {Actions.EndOfBlackScreenPhase?.Invoke();});
    }
}
