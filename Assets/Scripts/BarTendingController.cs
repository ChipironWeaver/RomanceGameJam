using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BarTendingController : MonoBehaviour
{
    public UnityEvent endShiftEvent = new UnityEvent(); 
    
    [Header("References")] 
    [SerializeField] private LiquidController _liquidController;
    [SerializeField] private GameObject _drinkParentModel;
    [SerializeField] private GameObject _drinkPanel;
    [SerializeField] private GameObject _decorationPanel;
    [SerializeField] private TextMeshProUGUI _hintText;
    [SerializeField] private UIDrinkReferences _ratingPanel;
    [SerializeField] private Button _emptyButton;
    [SerializeField] private Button _completeButton;
    [SerializeField] private UIAnimator _uiAnimator;

    [Header("Camera")] 
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 _baseCameraRotation;
    [SerializeField] private Vector3 _gameplayCameraRotation;
    [SerializeField] private float _baseFOV;
    [SerializeField] private float _gameplayFOV;
    [SerializeField] private Ease _cameraEase;
    [SerializeField] private float _cameraMoveTime;
    
    [Header("Decoration")] 
    [SerializeField] private List<DecorationGroup> _decorationGroups = new List<DecorationGroup>();
    [SerializeField] private GameObject _decorationGroupPanelPrefab;
    [SerializeField] private GameObject _decorationPrefab;
    [SerializeField] private Color _uiSelectedColor;
    [SerializeField] private Color _uiUnSelectedColor;
    [SerializeField] private Sprite _uiRemoveGroupSprite;

    [Header("Drink")] 
    [SerializeField] private GameObject _singularDrinkUiPrefab;
    [SerializeField] private List<Liquid> _liquids = new List<Liquid>();

    [Header("Gameplay")] 
    [SerializeField] private float _maxDay;
    [SerializeField,MinMaxSlider(0.0f, 20.0f)] private Vector2 _clientAmountRange;
    [SerializeField,CurveRange(0,0,1,100,EColor.Red)] private AnimationCurve _numberOfCustomerPerDayCurve;
    [SerializeField] private int _maxLiquids = 3;
    [SerializeField] private List<Recipe> _recipes;
    [SerializeField] private float _liquidScoreMultiplier = 1f;
    [SerializeField] private float _decorationScoreMultiplier = 1f;
    [SerializeField,CurveRange(0,0,1,100,EColor.Red)] private AnimationCurve _angryThreshold;
    [SerializeField,CurveRange(0,0,1,100)] private AnimationCurve _happyThreshold;

    [Header("Review Graphism")] 
    [SerializeField] private Sprite _angrySprite;
    [SerializeField] private Sprite _happySprite;
    [SerializeField] private Sprite _neutralSprite;
    [SerializeField] private Color _neutralColor;
    [SerializeField] private Color _happyColor;
    [SerializeField] private Color _angryColor;

    [Header("Current Unlocked")] 
    [SerializeField] private int _currentUnlockedDrinkAmount = -1;
    [SerializeField] private int _currentUnlockedDecorationAmount = -1;

    [Header("Characters")] 
    [SerializeField] private GameObject _daria;
    [SerializeField] private GameObject _angelina;
    [SerializeField] private GameObject _karin;
    [SerializeField] private List<GameObject> _npcs;
    
    private int _currentLiquidAmount;
    private List<int> _activeLiquidGroups = new List<int>();
    private int _currentGameState;
    private List<int> _activeDecorationGroups = new List<int>();
    private List<GameObject> _drinkUiList = new List<GameObject>();
    private List<GameObject> _decorationUiList = new List<GameObject>();

    private int _amountOfClientLeft = 0;
    private float _currentDay = 1;
    private MainCharacters _currentCharacter = MainCharacters.None;
    private GameObject _currentCharacterObject;
    private Recipe _currentRecipe;
    

    public void Start()
    {
        GameplayStart( 1, MainCharacters.None);
    }

    [Button]
    public void CreateUI()
    {
        if(_currentUnlockedDrinkAmount == -1) for (int i = 0; i < _liquids.Count; i++) CreateLiquidButton(i);
        else for (int i = 0; i < Mathf.Min(_liquids.Count,_currentUnlockedDrinkAmount); i++) CreateLiquidButton(i);
        
        if(_currentUnlockedDecorationAmount == -1) for (int i = 0; i < _decorationGroups.Count; i++) CreateDecorationButton(i);
        else for (int i = 0; i < Mathf.Min(_decorationGroups.Count,_currentUnlockedDecorationAmount); i++) CreateDecorationButton(i);
    }

    public void DoCameraMove(bool toGameplay)
    {
        _camera.transform.DORotate(toGameplay?_gameplayCameraRotation : _baseCameraRotation,_cameraMoveTime).SetEase(_cameraEase);
        _camera.DOFieldOfView(toGameplay?_gameplayFOV : _baseFOV,_cameraMoveTime).SetEase(_cameraEase);
    }
    
    public void DeleteUI()
    {
        foreach(GameObject ui in _drinkUiList) Destroy(ui);
        _drinkUiList.Clear();
        foreach(GameObject ui in _decorationUiList) Destroy(ui);
        _decorationUiList.Clear();
    }
    
    public void GameplayStart(float day, MainCharacters characters = MainCharacters.None)
    {
        if(_currentGameState > 0) return;
        if(characters == MainCharacters.None)
        {
            _amountOfClientLeft = (int)ChipironUtility.EvaluateVector2(_clientAmountRange, _numberOfCustomerPerDayCurve.Evaluate(day / _maxDay));
            if(_amountOfClientLeft <= 0) return;
        }
        else _amountOfClientLeft = 1;
        _currentDay = day;
        _currentCharacter = characters;
        CreateUI();
        ResetDrink();
        _currentGameState = 1;
        NextGameplayPhase();
    }

    [Button]
    public void NextGameplayPhase()
    {
        switch (_currentGameState)
        {
            case 1 : //null to drink
                _currentRecipe = _recipes[0]; // Need to randomize the recipes
                _uiAnimator.Fade(4);
                _uiAnimator.Fade(0);
                _hintText.text = _currentRecipe.hint;
                
                _currentGameState++;
                DoCameraMove(true);
                break;
            case 2 : //drink to decoration
                if (_currentLiquidAmount == _maxLiquids)
                {
                    _uiAnimator.Fade(1);
                    _uiAnimator.FadeOut(0);
                    _uiAnimator.FadeOut(2);
                    _uiAnimator.FadeOut(4);
                    _currentGameState++;
                }
                break;
            case 3 : //decoration to review
                _uiAnimator.FadeOut(1);
                _uiAnimator.FadeOut(3);
                Rate();
                _uiAnimator.Fade(5);
                DoCameraMove(false);
                _currentGameState++;
                break;
            case 4 : 
                _uiAnimator.FadeOut(5);
                _amountOfClientLeft--;
                ResetDrink();
                if (_amountOfClientLeft == 0)
                {
                    DeleteUI();
                    _currentGameState = -1;
                }
                else
                {
                    _currentGameState = 1;
                    NextGameplayPhase();
                }
                break;
        }
    }

    private void Rate()
    {
        float happyScore =  _happyThreshold.Evaluate(_currentDay/_maxDay);
        float angryScore = _angryThreshold.Evaluate(_currentDay/_maxDay);
        print(happyScore + " > " + angryScore);

        float score  = RateRecipe(_activeDecorationGroups,_activeLiquidGroups,_currentRecipe);

        _ratingPanel.nameText.text = score.ToString("N0") + " %";
        
        if (score > happyScore)
        {
            if(_happySprite)
            {
                _ratingPanel.image.sprite = _happySprite;
                _ratingPanel.image.color = Color.white;
            }
            else
            {
                _ratingPanel.image.sprite = null;
                _ratingPanel.image.color = _happyColor;
            }
            _ratingPanel.nameText.color = _happyColor;
        }
        else if (score < angryScore)
        {
            if(_angrySprite)
            {
                _ratingPanel.image.sprite = _angrySprite;
                _ratingPanel.image.color = Color.white;
            }
            else
            {
                _ratingPanel.image.sprite = null;
                _ratingPanel.image.color = _angryColor;
            }
            _ratingPanel.nameText.color = _angryColor;
        }
        else
        {
            if(_neutralSprite)
            {
                _ratingPanel.image.sprite = _neutralSprite;
                _ratingPanel.image.color = Color.white;
            }
            else
            {
                _ratingPanel.image.sprite = null;
                _ratingPanel.image.color = _neutralColor;
            }
            _ratingPanel.nameText.color = _neutralColor;
        }
    }

    private void FadeCharacter(MainCharacters character, bool isFadeIn)
    {
        
    }
    
    private void CreateLiquidButton(int index)
    {
        Liquid liquid = _liquids[index];
        GameObject liquidUI = Instantiate(_singularDrinkUiPrefab, _drinkPanel.transform);
        liquidUI.name = index + liquid.name ;
        UIDrinkReferences uiDrinkReferences = liquidUI.GetComponent<UIDrinkReferences>();
        uiDrinkReferences.nameText.text = liquid.name;
        uiDrinkReferences.button.onClick.AddListener(() => AddLiquidIndex(index));
        if(liquid.sprite != null)
        {
            uiDrinkReferences.image.sprite = liquid.sprite;
        }
        else
        {
            uiDrinkReferences.image.sprite = null;
            uiDrinkReferences.image.color = liquid.color;
        }
        _drinkUiList.Add(liquidUI);
    }
    private void CreateDecorationButton(int index)
    {
        GameObject group = Instantiate(_decorationGroupPanelPrefab, _decorationPanel.transform);
        group.name = "Decoration Panel " + _decorationGroups[index].name + " at " + index;
        UIDrinkReferences uiGroupReferences = group.GetComponent<UIDrinkReferences>();
        uiGroupReferences.nameText.text = _decorationGroups[index].name;

        GameObject removeGroupButton = Instantiate(_decorationPrefab, uiGroupReferences.groupChild.transform);
        removeGroupButton.name = "Remove Decoration Group Button";
        UIDrinkReferences  uiRemoveGroupReference = removeGroupButton.GetComponent<UIDrinkReferences>();
        if(_uiRemoveGroupSprite) uiRemoveGroupReference.image.sprite = _uiRemoveGroupSprite;
        else uiRemoveGroupReference.image.color = Color.crimson;
        uiRemoveGroupReference.button.onClick.AddListener(() => RemoveDecorationGroup(index));
            
        for (int y = 0; y < _decorationGroups[index].decorations.Count; y++)
        {
            GameObject decoration = Instantiate(_decorationPrefab, uiGroupReferences.groupChild.transform);
            decoration.name = "Decoration: " + _decorationGroups[index].decorations[y].name + " at " + y;
            UIDrinkReferences  uiDecoReference = decoration.GetComponent<UIDrinkReferences>();
            _decorationGroups[index].decorations[y].linkedImage = uiDecoReference.backGround;

            if (_decorationGroups[index].decorations[y].sprite)
                uiDecoReference.image.sprite = _decorationGroups[index].decorations[y].sprite;

            int y1 = y;
            uiDecoReference.button.onClick.AddListener(() => ShowDecoration(FindGroupCount(index) + y1));
            _decorationUiList.Add(decoration);
        }
    }
    public void AddLiquidIndex(int index)
    {
        if(_currentGameState != 2 | _currentLiquidAmount == _maxLiquids) return;
        
        if(!_emptyButton.gameObject.activeSelf) _uiAnimator.Fade(2);
        
        bool result = _liquidController.AddLiquidFromIndex(index);
        if (result)
        {
            _currentLiquidAmount++;
            if (_currentLiquidAmount == _maxLiquids)
            {
                _completeButton.interactable = true;
                _uiAnimator.FadeOut(2);
                _uiAnimator.Fade(3);
                //fade
            }
            _activeLiquidGroups.Add(index);
        }
    }

    public void Empty(bool force = false)
    {
        if(_currentGameState > 2 && ! force) return;
        _activeLiquidGroups.Clear();
        _uiAnimator.FadeOut(2);
        _currentLiquidAmount = 0;
        _completeButton.interactable = false;
        _liquidController.Empty();
    }

    public void ShowDecoration(int index)
    {
        if(_currentGameState != 3) return;
        
        int groupIndex = FindGroupIndex(index);
        if (index == -1) return;
        DecorationGroup group = _decorationGroups[groupIndex];
        Decoration decoration = group.decorations[index - FindGroupIndex(index,true)];
        
        
        if(decoration.linkedImage) decoration.linkedImage.color = _uiSelectedColor;
        
        if(_activeDecorationGroups[groupIndex] != -1) foreach(int y in _decorationGroups[groupIndex].decorations[_activeDecorationGroups[groupIndex]].optionToDisable) _decorationUiList[y].SetActive(true);
        
        else foreach (int y in group.noDecorationState.optionToDisable) _decorationUiList[y].SetActive(true);
        
        foreach(int y in decoration.optionToDisable)
        {
            _decorationUiList[y].SetActive(false);
            if (y - FindGroupIndex(y,true) == _activeDecorationGroups[FindGroupIndex(y)])
            {
                RemoveDecorationGroup(FindGroupIndex(y));
            }
        }
        
        _activeDecorationGroups[groupIndex] = index - FindGroupIndex(index,true);
        
        foreach (Decoration dec in group.decorations)
        {
            if (dec.linkedImage) dec.linkedImage.color = dec == decoration ? _uiSelectedColor : _uiUnSelectedColor;
            
            foreach (GameObject obj in dec.decorationObject)
            {
                obj.SetActive(decoration.decorationObject.Contains(obj));
            }
        }
        foreach (var dec in _decorationGroups[groupIndex].noDecorationState.decorationObject)
        {
            dec.SetActive(decoration.decorationObject.Contains(dec));
        }
        
        print(ChipironUtility.GetListString(_activeDecorationGroups));
    }

    public void RemoveDecorationGroup(int index,List<int> blackList = null)
    {
        if (_currentUnlockedDecorationAmount != -1 && index < _currentUnlockedDecorationAmount) return;
        
        if (_decorationGroups.Count > index)
        {
            foreach (Decoration decoration in _decorationGroups[index].decorations)
            {
                if(decoration.linkedImage) decoration.linkedImage.color = _uiUnSelectedColor;
                foreach (var dec in decoration.decorationObject)
                {
                    dec.SetActive(false);
                }
            }
            
            foreach (var dec in _decorationGroups[index].noDecorationState.decorationObject)
            {
                dec.SetActive(true);
            }
            if(_activeDecorationGroups[index] != -1) foreach(int i in _decorationGroups[index].decorations[_activeDecorationGroups[index]].optionToDisable) _decorationUiList[i].SetActive(true);
            if(_decorationGroups[index].noDecorationState.linkedImage) _decorationGroups[index].noDecorationState.linkedImage.color = _uiUnSelectedColor;
            
            _activeDecorationGroups[index] = -1;
            foreach (int y in _decorationGroups[index].noDecorationState.optionToDisable)
            {
                if(y < _decorationUiList.Count)
                {
                    _decorationUiList[y].SetActive(false);
                    int groupIndex = FindGroupIndex(y);
                    blackList ??= new List<int>();
                    if (!blackList.Contains(y))
                    {
                        blackList.Add(y);
                        RemoveDecorationGroup(groupIndex, blackList);
                    }
                }
            }
        }
    } 

    public float RateRecipe(List<int> decoration,List<int> liquids, Recipe recipe)
    {
        print("Décoration : " + ChipironUtility.GetListString(decoration));
        print("Liquids : " + ChipironUtility.GetListString(liquids));
        float liquidScore = 0;
        float liquidTotalScore = 0;
        float decorationScore = 0;
        float decorationTotalScore = 0;

        recipe.liquids.Sort();
        liquids.Sort();
        
        for (int i = 0 ; i < recipe.liquids.Count ; i++)
        {
            if (recipe.liquids[i] != -1)
            {
                liquidTotalScore++;
                if (liquids.Count > i)
                {
                    if(recipe.liquids[i] == liquids[i]) liquidScore++;
                }
            }
        }
        
        for (int i = 0; i < Mathf.Min(recipe.decorations.Count, decoration.Count ); i++)
        {
            if (recipe.decorations[i].validIndexes.Count == 0)
            {
                if (recipe.decorations[i].hasToBeEmpty)
                {
                    decorationTotalScore++;
                    if (decoration[i] == -1) decorationScore++;
                } 
            }
            else
            {
                decorationTotalScore++;
                if (recipe.decorations[i].validIndexes.Contains(decoration[i])) decorationScore++;
            }
        }
        print(liquidScore + " / " + liquidTotalScore + " | " + decorationScore +  " / " + decorationTotalScore);
        if (liquidTotalScore == 0) return -1;
        return (liquidScore / liquidTotalScore * _liquidScoreMultiplier + decorationScore / decorationTotalScore * _decorationScoreMultiplier) / (_liquidScoreMultiplier + _decorationScoreMultiplier) *100;
    }
    
    public int FindGroupIndex(int index, bool isPosition = false)
    {
        int indexSearch = 0;
        for (int i = 0; i < _decorationGroups.Count; i++)
        {
            if (indexSearch + _decorationGroups[i].decorations.Count > index)
            {
                return isPosition? indexSearch : i;
            }
            indexSearch += _decorationGroups[i].decorations.Count;
        }
        return -1;
    }
    
    public int FindGroupCount(int index)
    {
        int indexSearch = 0;
        for (int i = 0; i < _decorationGroups.Count; i++)
        {
            if (i == index)
            {
                return indexSearch;
            }
            indexSearch += _decorationGroups[i].decorations.Count;
        }
        return -1;
    }

    public void ResetDrink()
    {
        _activeDecorationGroups.Clear();
        foreach (var bob in _decorationGroups)
        {
            _activeDecorationGroups.Add(-1);
        }
        for (int i = 0; i < _decorationGroups.Count; i++)
        {
            RemoveDecorationGroup(i);
        }
        _liquidController.liquids = _liquids;
        _liquidController.sizePerLiquid = 1f / _maxLiquids;
        _completeButton.interactable = false;
        Empty(true);
    }
}

[Serializable]
public class DecorationGroup
{
    public string name;
    public Sprite sprite;
    public Decoration noDecorationState;
    public List<Decoration> decorations = new List<Decoration>();
}
[Serializable]
public class Decoration
{
    public string name;
    public List<GameObject> decorationObject = new List<GameObject>();
    public List<int> optionToDisable = new List<int>();
    public Image linkedImage;
    public Material material;
    public Sprite sprite;
    public bool hasColorOptions;
    [ShowIf("hasColorOptions")] public List<Material> colorOptions = new List<Material>();
    [ShowIf("hasColorOptions")] public List<Sprite> customColorSprites = new List<Sprite>();
}

[Serializable]
public class Recipe
{
    public string name;
    public int day;
    public string hint;
    public MainCharacters characters;
    public List<int> liquids;
    public List<RecipeComponent> decorations; 
}

[Serializable]
public class RecipeComponent
{
    public bool hasToBeEmpty;
    public List<int> validIndexes;
}