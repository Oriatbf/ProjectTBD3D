using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : Singleton<ApplicationManager>
{
    private Dictionary<Type, BaseController> _modulesByType = new Dictionary<Type, BaseController>();
    private void Awake()
    {
        _modulesByType.Add(typeof(CanvasController), new CanvasController());
        _modulesByType.Add(typeof(SkillTurnCounterController), new SkillTurnCounterController());
        _modulesByType.Add(typeof(TurnController), new TurnController());
        _modulesByType.Add(typeof(EnemyRegisterController),new EnemyRegisterController());
        _modulesByType.Add(typeof(CharacterInfoController), new CharacterInfoController());
        _modulesByType.Add(typeof(PopUpUIController), new PopUpUIController());
        _modulesByType.Add(typeof(SkillStackController),new SkillStackController());
        _modulesByType.Add(typeof(PlayerSpawnController),new PlayerSpawnController());
        _modulesByType.Add(typeof(LootController), new LootController());
        _modulesByType.Add(typeof(InformationController), new InformationController());
       // _modulesByType.Add(typeof(ShopController), new ShopController());
        _modulesByType.Add(typeof(BuffStackController), new BuffStackController());
        _modulesByType.Add(typeof(BuffInfoController), new BuffInfoController());
        _modulesByType.Add(typeof(TopInfoController), new TopInfoController());
        _modulesByType.Add(typeof(SkillChangeController), new SkillChangeController());
        _modulesByType.Add(typeof(CameraController), new CameraController());

    }
    private void Start()
    {
        foreach (var value in _modulesByType.Values)
        {
            value.OnInitialize();
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
