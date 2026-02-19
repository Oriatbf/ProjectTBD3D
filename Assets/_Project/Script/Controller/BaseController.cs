using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public struct ControllerInfo
{
    public string[] ContainSceneNames;
    public int Priority;
    public int UpdateInterval;
    public int LateUpdateInterval;
    public int FixedUpdateInterval;
}

public abstract class BaseController
{
    public virtual ControllerInfo ControllerInfo { get; } = new()
    {
        ContainSceneNames = new string[] {"GamePlay" },
        Priority = 0,
        UpdateInterval = 1,
        LateUpdateInterval = 1,
        FixedUpdateInterval = 1,
    };
   
    public virtual void OnInitialize()
    {
        
    }
    
    public virtual void OnUpdate()
    {
        
    }

    public virtual void OnLateUpdate()
    {
        
    }

    public virtual void OnFixedUpdate()
    {
        
    }
}
