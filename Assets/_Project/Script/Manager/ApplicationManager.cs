using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : Singleton<ApplicationManager>
{
    private Dictionary<Type, BaseController> _modulesByType = new Dictionary<Type, BaseController>();
    private void Awake()
    {
        _modulesByType.Add(typeof(SkillTurnCounterController), new SkillTurnCounterController());
        _modulesByType.Add(typeof(TurnController), new TurnController());
        _modulesByType.Add(typeof(EnemyRegisterController),new EnemyRegisterController());
    }
    private void Start()
    {
        foreach (var value in _modulesByType.Values)
        {
            value.OnIntialize();
        }
    }

    private void Update()
    {
        foreach (var value in _modulesByType.Values)
        {
            value.OnUpdate();
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
