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
        ChainSawContract,GoldToHeal,Gamble
    }
    
    private Animator _animator;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private Dialogue dialogue;
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

    private void SetHealToGold()
    {
        var topInfoUIController = ApplicationManager.Inst.GetModule<TopInfoController>();
        if (topInfoUIController.GetTopInfoCanvas().GetGold() < 300) return;
        topInfoUIController.AddGold(-300);
        var savedUnits = DataManager.Inst.GetAllSavedUnits();
        foreach (var unitSaveData in savedUnits)
        {
            unitSaveData.statContainer.hp.AddBaseValue(3);
        }
    }

    private void Gamble()
    {
        if (Random.value < 0.5f)
        {
            dialogue.SetTxt("멍청아");
            var allUnitDatas = DataManager.Inst.GetAllSavedUnits();
            var randomUnitData = allUnitDatas[Random.Range(0, allUnitDatas.Count)];
            if (allUnitDatas.Count == 1)
            {
                FactoryManager.Inst.Lose();
            }
            DataManager.Inst.DeleteUnit(randomUnitData.constId);
            
        }
        else
        {
            var unitData = SheetDataManager.Inst.GetRandomUnitData(1);
            var unitSaveData = new UnitSaveData(unitData[0]);
            DataManager.Inst.SaveUnit(unitSaveData);
            dialogue.SetTxt("운이 좋네");
        }
    }

    public void SetEvent()
    {
        var randomEvent = Extensions.GetRandomEnum<DemonEvent>();
         dialogue = ApplicationManager.Inst.GetModule<PoolController>().Spawn<Dialogue>("Dialogue");
        if(dialogue==null)Debug.LogError("dialogue pool is null");
        EventDialogueSO eventDialogueSO = null;
        switch (randomEvent)
        {
            case DemonEvent.ChainSawContract:
                eventDialogueSO = Resources.Load<EventDialogueSO>("SO/Event/DemonEventDialogue");
                eventDialogueSO.SetEventAction(SetChainSawDemon);
                break;
            case DemonEvent.GoldToHeal:
                eventDialogueSO = Resources.Load<EventDialogueSO>("SO/Event/HealEventDialogue");
                eventDialogueSO.SetEventAction(SetHealToGold);
                break;
            case DemonEvent.Gamble:
                eventDialogueSO = Resources.Load<EventDialogueSO>("SO/Event/GambleEventDialogue");
                eventDialogueSO.SetEventAction(Gamble);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        dialogue.SetEventDialogue(eventDialogueSO);
    }
    
    
    
}
