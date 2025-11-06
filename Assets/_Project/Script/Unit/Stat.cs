using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Stat
{
    public delegate void OnValueChangeDelegate(float value);
    public event OnValueChangeDelegate OnValueChanged;
    public float _baseValue; //기본 값
    public float _originalValue; //원본 값
    public float _maxValue; // 최대값
    public bool _isInfiniteValue = false; //스탯이 무한정하게 늘어날 수 있는가
    public static Stat Create(float baseValue = 0)
    {
        return Create(baseValue,  true);
    }

    public static Stat Create(float baseValue = 0, float maxValue = 0)
    {
        return Create(baseValue, false,maxValue);
    }
    
    private static Stat Create(float baseValue = 0,bool isInfinite = false,float maxValue = 0)
    {
        var stat = new Stat
        {
            _baseValue = baseValue,
            _isInfiniteValue = isInfinite,
            _originalValue = baseValue,
            _maxValue = maxValue
        };
        return stat;
    }

    public void AddBaseValue(float value)
    {
        _baseValue += value;
        ChangeValueHandler();
    }
    
    public void SetBaseValue(float value)
    {
        _baseValue = value;
        ChangeValueHandler();
    }
    

    public void SetOriginalValue(float value) => _originalValue = value;
    public void AddOriginalValue(float value)=>  _originalValue += value;
    public void SetMaxValue(float value) => _maxValue = value;
    public void AddMaxValue(float value) => _maxValue += value;

    
    private void ChangeValueHandler()
    {
        /*
        if(_baseValue >= _originalValue && !_isInfiniteValue)
            _baseValue = _originalValue;
        */
        if(_baseValue >= _maxValue && !_isInfiniteValue)_baseValue = _maxValue;
        if (_baseValue < 0) _baseValue = 0;
        OnValueChanged?.Invoke(_baseValue);
    }
}
