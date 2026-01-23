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
    

    private void SetChainSawDemon()
    {
        Debug.Log("체인소맨으로 변경");
        var savedUnits = DataManager.Inst.Data.units;
        var randomIndex = Random.Range(0, savedUnits.Count);
        var targetUnit = savedUnits[randomIndex];
        targetUnit.bringSkills = new List<int>() { 22 };
    }


    public void SetEvent()
    {
        var randomEvent = Extensions.GetRandomEnum<DemonEvent>();
        EventDialogueSO eventDialogueSO = null;
        switch (randomEvent)
        {
            case DemonEvent.ChainSawContract:
                eventDialogueSO = Resources.Load<EventDialogueSO>("SO/DemonEventDialogue");
                var dialogue = ApplicationManager.Inst.GetModule<PoolController>().Spawn<Dialogue>("Dialogue");
                if(dialogue==null)Debug.LogError("dialogue pool is null");
                eventDialogueSO.SetEventAction(SetChainSawDemon);
                dialogue.SetEventDialogue(eventDialogueSO);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    
}
