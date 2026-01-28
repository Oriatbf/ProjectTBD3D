using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PlayerSpawnCanvas : BaseCanvas
{
    [SerializeField ]private Transform instanceParent;
    [SerializeField] private RectTransform backGround;
    [SerializeField] private Button spawnEndBtn;
    [SerializeField] private UnitIcon unitIconPrefab;
  


    public void  Init(List<UnitSaveData> unitSaveData)
    {
        foreach (var unitData in unitSaveData)
        {
            var instance = Instantiate(unitIconPrefab,instanceParent);
            instance.Init(unitData);
        }
    }

    public void SetSpawnEndAction(Action action)
    {
        Debug.Log("SpawnEnd");
        spawnEndBtn.onClick.AddListener(()=>action?.Invoke());
    }

  
    
}