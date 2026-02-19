using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public readonly struct StatModifier
{
    public readonly EStatModifier ModifierType;
    public readonly float Value;

    public StatModifier(EStatModifier modifierType, float value)
    {
        ModifierType = modifierType;
        Value = value;
    }
}

[Serializable]
public class StatModifierCollection
{
    [Serializable]
    private struct ModifierEntry
    {
        public float Value;
        public bool IsActive;

        public ModifierEntry(float value)
        {
            Value = value;
            IsActive = true;
        }
    }
    
    private ModifierEntry[] _entries;
    private int _count;
    private int _capacity;

    public StatModifierCollection(int initialCapacity)
    {
        _entries = new ModifierEntry[initialCapacity];
        _count = 0;
        _capacity = initialCapacity;
    }

    // Lazy initialization - null 체크 시 자동 초기화
    private void EnsureInitialized()
    {
        if (_entries == null || _entries.Length ==0)
        {
            _entries = new ModifierEntry[4];
            _capacity = 4;
            _count = 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(float value)
    {
        Debug.Log("add");
        EnsureInitialized();
        Debug.Log(_entries.Length);
        Debug.Log(_count);
        // 용량 초과 시 리사이즈
        if (_count >= _capacity)
        {
            Resize();
        }

        _entries[_count++] = new ModifierEntry(value);
    }

    private void Resize()
    {
        var newCapacity = _capacity * 2;
        var newEntries = new ModifierEntry[newCapacity];
        
        Array.Copy(_entries, newEntries, _capacity);
        
        _entries = newEntries;
        _capacity = newCapacity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(int index)
    {
        EnsureInitialized();
        
        if (index < 0 || index >= _count)
            return false;

        _entries[index].IsActive = false;
        
        // 배열 끝부분이면 카운트 감소
        if (index == _count - 1)
        {
            _count--;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        EnsureInitialized();
        
        for (int i = 0; i < _count; i++)
        {
            _entries[i].IsActive = false;
        }

        _count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetSum()
    {
        EnsureInitialized();
        
        var sum = 0f;
        Debug.Log($" count : {_count}  length : {_entries.Length}");
        for (int i = 0; i < _count; i++)
        {
            if (_entries[i].IsActive)
            {
                sum += _entries[i].Value;
            }
        }

        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetMultiplicationFactor()
    {
        EnsureInitialized();
        
        var factor = 1f;
        for (int i = 0; i < _count; i++)
        {
            if (_entries[i].IsActive)
            {
                factor *= _entries[i].Value;
            }
        }

        return factor;
    }

    public int Count 
    { 
        get 
        { 
            EnsureInitialized();
            return _count; 
        } 
    }
}