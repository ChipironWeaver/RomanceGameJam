using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BarTendingController _barTendingController;
    [SerializeField] private GameObject _recipeBookPanel; 
    [SerializeField] private Image _fadeImage;
    [SerializeField] private List<GameObject> _recipesObjects = new List<GameObject>();
    [SerializeField] private List<int> _recipesPerDay = new List<int>();
    
    [Header("Animation")]
    [SerializeField] private float _panelShowTime;
    [SerializeField] private float _recipeShowTime;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _transColor;

    private int _currentDay;
    private int _currentIndex;
    private int _previousIndex;
    private bool _isShown;
    private List<GameObject> _activeRecipesObjects;
    private bool _isShowingAnimation;
    private bool _isShowingRecipe;

    public void SetActive(bool active)
    {
        if(active == _isShown) return;
        if (_isShowingAnimation) return;
        if (active)
        {
            _currentDay = (int)_barTendingController.currentDay;
            _activeRecipesObjects = _recipesObjects;
            if (!(_currentDay >= _recipesPerDay.Count))
            {
                _activeRecipesObjects.RemoveRange(_recipesPerDay[_currentDay], _activeRecipesObjects.Count - _recipesPerDay[_currentDay]);
            }
        }
        _isShown = active;
        _isShowingAnimation = true;
        _recipeBookPanel.transform.DOScaleX(active ? 1 : 0,_panelShowTime).SetEase(active ? Ease.OutSine : Ease.InSine).OnComplete((() => { _isShowingAnimation = false; }));
    }

    public void Next(bool inverted)
    {
        if (_isShowingAnimation) return;
        _previousIndex = _currentIndex;
        _currentIndex += inverted ? -1 : 1;
        if(_currentIndex < 0) _currentIndex = _activeRecipesObjects.Count - 1;
        else if(_currentIndex >= _activeRecipesObjects.Count) _currentIndex = 0;
        _isShowingAnimation = true;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_fadeImage.DOColor(_transColor, _recipeShowTime));
        sequence.AppendCallback(() => {_activeRecipesObjects[_previousIndex].SetActive(false); _activeRecipesObjects[_currentIndex].SetActive(true); });
        sequence.Append(_fadeImage.DOColor(_baseColor, _recipeShowTime));
        sequence.OnComplete(() => { _isShowingAnimation = false; });
    }
}
