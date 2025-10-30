using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    private Dictionary<Type, BaseController> _modulesByType = new Dictionary<Type, BaseController>();
    private void Awake()
    {
        DIContainer.RegisterService(this);
        _modulesByType.Add(typeof(SkillTurnCounterController), new SkillTurnCounterController());
    }
    private void Start()
    {
        foreach (var value in _modulesByType.Values)
        {
            value.OnIntialize();
        }
    }
    
    public T GetModule<T>() where T : BaseController
    {
        if (_modulesByType.TryGetValue(typeof(T), out var module))
        {
            return module as T;
        }

        return null;
    }

    
    public BaseController GetModule(Type moduleType) => _modulesByType[moduleType];
}
