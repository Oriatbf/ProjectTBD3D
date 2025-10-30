using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Stat
{
    public delegate void OnValueChangeDelegate(float value);
    public event OnValueChangeDelegate OnValueChanged;
    public float _baseValue;
    public float _originalValue;
    public float _maxValue;
    public bool _isInfiniteValue = false;
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
            _baseValue = baseValue
        };
        stat._isInfiniteValue = isInfinite;
        if(!isInfinite)stat._maxValue = maxValue;
        stat._originalValue = stat._baseValue;
        return stat;
    }

    public void AddBaseValue(float value)
    {
        _baseValue += value;
        ChangeValueHandler();
    }

    public void SetOriginalValue(float value)
    {
        _originalValue = value;
    }

    public void SetBaseValue(float value)
    {
        _baseValue = value;
        ChangeValueHandler();
    }
    
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
