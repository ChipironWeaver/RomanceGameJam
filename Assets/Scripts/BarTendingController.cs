using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class BarTendingController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private LiquidController _liquidController;
    [SerializeField] private GameObject _drinkParentModel;
    [SerializeField] private GameObject _drinkPanel;
    [SerializeField] private GameObject _decorationPanel;
    [SerializeField] private Button _emptyButton;
    [SerializeField] private Button _completeButton;
    [SerializeField] private UIAnimator _uiAnimator;

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
    [SerializeField] private int _maxLiquids = 3;
    [SerializeField] private string _recipes;
    
    private int _currentLiquidAmount;
    private int _currentGameState;
    private List<int> _activeDecorationGroups = new List<int>();
    private List<GameObject> _drinkUiList = new List<GameObject>();
    private List<GameObject> _decorationUiList = new List<GameObject>();
    
    public void Start()
    {
        
        _liquidController.liquids = _liquids;
        _liquidController.sizePerLiquid = 1f / _maxLiquids;
        _completeButton.interactable = false;
        
        CreateUI();
        ResetDrink();
    }
    
    [Button]
    public void CreateUI()
    {
        for (int i = 0; i < _liquids.Count; i++)
        {
            CreateLiquidButton(i);
        }

        int currentGroupIndex = 0;
        for (int i = 0; i < _decorationGroups.Count; i++)
        {
            GameObject group = Instantiate(_decorationGroupPanelPrefab, _decorationPanel.transform);
            group.name = "Decoration Panel " + _decorationGroups[i].name + " at " + i;
            UIDrinkReferences uiGroupReferences = group.GetComponent<UIDrinkReferences>();
            uiGroupReferences.nameText.text = _decorationGroups[i].name;

            GameObject removeGroupButton = Instantiate(_decorationPrefab, uiGroupReferences.groupChild.transform);
            removeGroupButton.name = "Remove Decoration Group Button";
            UIDrinkReferences  uiRemoveGroupReference = removeGroupButton.GetComponent<UIDrinkReferences>();
            if(_uiRemoveGroupSprite) uiRemoveGroupReference.image.sprite = _uiRemoveGroupSprite;
            else uiRemoveGroupReference.image.color = Color.crimson;
            var i1 = i;
            uiRemoveGroupReference.button.onClick.AddListener(() => RemoveDecorationGroup(i1));
            
            for (int y = 0; y < _decorationGroups[i].decorations.Count; y++)
            {
                GameObject decoration = Instantiate(_decorationPrefab, uiGroupReferences.groupChild.transform);
                decoration.name = "Decoration: " + _decorationGroups[i].decorations[y].name + " at " + y;
                UIDrinkReferences  uiDecoReference = decoration.GetComponent<UIDrinkReferences>();
                _decorationGroups[i].decorations[y].linkedImage = uiDecoReference.backGround;

                if (_decorationGroups[i].decorations[y].sprite)
                    uiDecoReference.image.sprite = _decorationGroups[i].decorations[y].sprite;
                var index = currentGroupIndex;
                var y1 = y;
                uiDecoReference.button.onClick.AddListener(() => ShowDecoration(index + y1));
                _decorationUiList.Add(decoration);
            }
            
            currentGroupIndex += _decorationGroups[i].decorations.Count;
        }
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
    
    public void AddLiquidIndex(int index)
    {
        if (_currentLiquidAmount == _maxLiquids)
        {
            _completeButton.interactable = true;
        }

        bool result = _liquidController.AddLiquidFromIndex(index);
        print(result);
        if (result) _currentLiquidAmount ++;
    }

    public void Empty()
    {
        _currentLiquidAmount = 0;
        _completeButton.interactable = false;
        _liquidController.Empty();
    }

    public void ShowDecoration(int index)
    {
        
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
                print("y");
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
            //Set null case as active
            foreach (var dec in _decorationGroups[index].noDecorationState.decorationObject)
            {
                dec.SetActive(true);
            }
            if(_activeDecorationGroups[index] != -1) foreach(int i in _decorationGroups[index].decorations[_activeDecorationGroups[index]].optionToDisable) _decorationUiList[i].SetActive(true);
            if(_decorationGroups[index].noDecorationState.linkedImage) _decorationGroups[index].noDecorationState.linkedImage.color = _uiUnSelectedColor;
            
            _activeDecorationGroups[index] = -1;
            foreach (int y in _decorationGroups[index].noDecorationState.optionToDisable)
            {
                _decorationUiList[y].SetActive(false);
                int groupIndex = FindGroupIndex(y);
                blackList ??= new List<int>();
                if(!blackList.Contains(y))
                {
                    blackList.Add(y);
                    RemoveDecorationGroup(groupIndex);
                }
            }
        }
        
        print(ChipironUtility.GetListString(_activeDecorationGroups));
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
        Empty();
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
    [ShowIf("hasColorOptions")] public List<Color> colorOptions = new List<Color>();
    [ShowIf("hasColorOptions")] public List<Sprite> customColorSprites = new List<Sprite>();
}