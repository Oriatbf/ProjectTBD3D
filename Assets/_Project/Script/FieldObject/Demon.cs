using System;
using System.Collections.Generic;
using _Project.Pooling;
using _Project.Script.Controller;
using _Project.Script.FieldObject;
using Core.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

public class Demon : FieldObject
{
    public enum DemonEvent
    {
        ChainSawContract,
    }
    
    private Animator _animator;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.Play(Idle);
        SetEvent();
    }

    private void OnMouseDown()
    {
        SetChainSawDemon();
    }

    private void SetChainSawDemon()
    {
        var savedUnits = DataManager.Inst.Data.units;
        var randomIndex = Random.Range(0, savedUnits.Count);
        var targetUnit = savedUnits[randomIndex];
        targetUnit.bringSkills = new List<int>() { 22 };
    }


    public void SetEvent()
    {
        var randomEvent = Extensions.GetRandomEnum<DemonEvent>();

        switch (randomEvent)
        {
            case DemonEvent.ChainSawContract:
                var d = ApplicationManager.Inst.GetModule<PoolController>().Spawn<Dialogue>("Dialogue");
                if(d==null)Debug.LogError("pool is null");
                d.SetTxt("난 악마다 히히");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    
}
