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

    [Header("Drink")] 
    [SerializeField] private GameObject _singularDrinkUiPrefab;
    [SerializeField] private List<Liquid> _liquids = new List<Liquid>();

    [Header("Gameplay")] 
    [SerializeField] private int _maxLiquids = 3;
    [SerializeField] private string _recipes;
    
    private int _currentLiquidAmount = 0;
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
    public bool usable;
    public GameObject decorationObject;
    public Material material;
    public Sprite sprite;
    public bool hasColorOptions;
    [ShowIf("hasColorOptions")] public List<Color> colorOptions = new List<Color>();
    [ShowIf("hasColorOptions")] public List<Sprite> customColorSprites = new List<Sprite>();
}