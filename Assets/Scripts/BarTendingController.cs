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
    
    public void Start()
    {
        Empty();
        _liquidController.liquids = _liquids;
        _liquidController.sizePerLiquid = 1f / _maxLiquids;
        _completeButton.interactable = false;
        CreateUI();
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

                if (_decorationGroups[i].decorations[y].sprite)
                    uiDecoReference.image.sprite = _decorationGroups[i].decorations[y].sprite;
                var index = currentGroupIndex;
                var y1 = y;
                uiDecoReference.button.onClick.AddListener(() => ShowDecoration(index + y1));
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
        //find the decoration group
        int indexSearch = 0;
        DecorationGroup group = null;
        Decoration decoration = null;
        for (int i = 0; i < _decorationGroups.Count; i++)
        {
            if (indexSearch + _decorationGroups[i].decorations.Count > index)
            {
                group = _decorationGroups[i];
                decoration = group.decorations[index - indexSearch];
                break;
            }

            indexSearch += _decorationGroups[i].decorations.Count;
        }
        
        print(group != null ? group.name : "No group found");
        print(decoration != null ? decoration.name : "No decoration found");
        if(decoration == null | group == null) return;
        
        foreach (Decoration dec in group.decorations)
        {
            if (dec == decoration)
            {
                if (!dec.decorationObject.activeSelf)
                {
                    if(dec.linkedImage) dec.linkedImage.color = _uiSelectedColor;
                    dec.decorationObject.SetActive(true);
                }
            }
            else
            {
                if (dec.decorationObject.activeSelf)
                {
                    if(dec.linkedImage) dec.linkedImage.color = _uiUnSelectedColor;
                    dec.decorationObject.SetActive(false);
                }
            }
        }
    }

    public void RemoveDecorationGroup(int index)
    {
        if (_decorationGroups.Count > index)
        {
            foreach (Decoration decoration in _decorationGroups[index].decorations)
            {
                print(decoration.name + decoration.decorationObject.activeSelf);
                if (decoration.decorationObject.activeSelf)
                {
                    if(decoration.linkedImage) decoration.linkedImage.color = _uiUnSelectedColor;
                    decoration.decorationObject.SetActive(false);
                }
            }
        }
    }

    public void ResetDrink()
    {
        for (int i = 0; i < _decorationGroups.Count; i++)
        {
            RemoveDecorationGroup(i);
        }
        Empty();
    }

    [Button]
    public void Test()
    {
        ShowDecoration(2);
        ShowDecoration(1);
    }
}

[Serializable]
public class DecorationGroup
{
    public string name;
    public Sprite sprite;
    public List<Decoration> decorations = new List<Decoration>();
}
[Serializable]
public class Decoration
{
    public string name;
    public bool usable = true;
    public GameObject decorationObject;
    public Image linkedImage;
    public Material material;
    public Sprite sprite;
    public bool hasColorOptions;
    [ShowIf("hasColorOptions")] public List<Color> colorOptions = new List<Color>();
    [ShowIf("hasColorOptions")] public List<Sprite> customColorSprites = new List<Sprite>();
}