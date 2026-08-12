using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LiquidController : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private List<Liquid> _liquids = new List<Liquid>();
    [Header("Animation Settings")]
    [SerializeField,MinMaxSlider(0,1)] private Vector2 _waveStrengthRange;
    [SerializeField,MinMaxSlider(0,1)] private Vector2 _liquidMinMaxHeight;
    [SerializeField,CurveRange(0, 0, 1, 1,EColor.Violet)] private AnimationCurve _waveStrengthCurve;
    [SerializeField,CurveRange(0, 0, 1, 1)] private AnimationCurve _animationEase;
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _sizePerLiquid;
    
    private Liquid _currentLiquid = new Liquid();
    private int _currentNumberOfLiquids;
    private Liquid _endLiquid = new Liquid();

    private Liquid _timedLiquid;
    private bool _isInAnimation;
    private float _animationTimer;

    public void Update()
    {
        if (_isInAnimation)
        {
            _animationTimer += Time.deltaTime;
            if (_animationTimer >= _animationDuration)
            {
                _animationTimer = 0f;
                _isInAnimation = false;
                _currentLiquid = _endLiquid;
                SetLiquids(_endLiquid,ChipironUtility.EvaluateVector2(_liquidMinMaxHeight, _currentNumberOfLiquids * _sizePerLiquid));
            }
            else
            {
                float currentEasedTime =  _animationEase.Evaluate(_animationTimer / _animationDuration) ;
                _timedLiquid = new Liquid()
                {
                    color = Color.Lerp(_currentLiquid.color, _endLiquid.color, currentEasedTime),
                    alpha = ChipironUtility.EvaluateFloat(_currentLiquid.alpha, _endLiquid.alpha, currentEasedTime),
                    smoothness = ChipironUtility.EvaluateFloat(_currentLiquid.smoothness, _endLiquid.smoothness, currentEasedTime),
                    hueShift = ChipironUtility.EvaluateFloat(_currentLiquid.hueShift, _endLiquid.hueShift, currentEasedTime),
                    valueShift = ChipironUtility.EvaluateFloat(_currentLiquid.valueShift, _endLiquid.valueShift, currentEasedTime),
                    saturationShift = ChipironUtility.EvaluateFloat(_currentLiquid.saturationShift, _endLiquid.saturationShift, currentEasedTime),
                };
                _material.SetFloat("_WaveStrenght", ChipironUtility.EvaluateVector2(_waveStrengthRange,_waveStrengthCurve.Evaluate(_animationTimer / _animationDuration)));
                SetLiquids(_timedLiquid, ChipironUtility.EvaluateVector2(_liquidMinMaxHeight,
                    ChipironUtility.EvaluateFloat((_currentNumberOfLiquids-1) * _sizePerLiquid ,_currentNumberOfLiquids * _sizePerLiquid , currentEasedTime)));
            }
        }
    }

    public void AddLiquid(Liquid liquid)
    {
        if (_isInAnimation) return;
        float liquidTime = 1 - _currentNumberOfLiquids / ((float)_currentNumberOfLiquids + 1);
        if (_currentNumberOfLiquids == 0)
        {
            _currentLiquid =  liquid;
        }
        _endLiquid = new Liquid
        {
            color = Color.Lerp(_currentLiquid.color, liquid.color, liquidTime),
            alpha = ChipironUtility.EvaluateFloat(_currentLiquid.alpha, liquid.alpha, liquidTime),
            smoothness = ChipironUtility.EvaluateFloat(_currentLiquid.smoothness, liquid.smoothness, liquidTime),
            hueShift = ChipironUtility.EvaluateFloat(_currentLiquid.hueShift, liquid.hueShift, liquidTime),
            valueShift = ChipironUtility.EvaluateFloat(_currentLiquid.valueShift, liquid.valueShift, liquidTime),
            saturationShift = ChipironUtility.EvaluateFloat(_currentLiquid.saturationShift, liquid.saturationShift, liquidTime),
        };
        _currentNumberOfLiquids++;
        _isInAnimation = true;
    }

    public void AddLiquidFromIndex(int liquidIndex)
    {
        AddLiquid(_liquids[liquidIndex]);
    }
    [Button]
    public void Empty()
    {
        SetLiquids(new Liquid(), 0);
        _currentNumberOfLiquids = 0;
        _isInAnimation = false;
    }
    
    private void SetLiquids(Liquid liquid, float height)
    {
        _material.SetFloat("_Height", height);
        _material.SetColor("_BaseColor", liquid.color);
        _material.SetFloat("_Alpha", height <= 0 ?  0f : liquid.alpha);
        _material.SetFloat("_Smoothness", liquid.smoothness);
        _material.SetFloat("_HueShift", liquid.hueShift);
        _material.SetFloat("_ValueShift", liquid.valueShift);
        _material.SetFloat("_SaturationShift", liquid.saturationShift);
    }


    [Button]
    private void Test()
    {
        SetLiquids(_liquids[0], 1f);
    }

    
}
[Serializable]
public class Liquid
{
    public string name;
    public Color color;
    public float alpha;
    public float smoothness;
    public float hueShift;
    public float valueShift;
    public float saturationShift;
}
