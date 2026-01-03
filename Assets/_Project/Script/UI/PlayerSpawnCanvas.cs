using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PlayerSpawnCanvas : MonoBehaviour
{
    [SerializeField ]private Transform parent;
    [SerializeField] private RectTransform backGround;
    [SerializeField] private Button spawnEndBtn;
    [SerializeField] private UnitIcon unitIconPrefab;
  


    public async void  Init(List<UnitSaveData> unitSaveData)
    {
        foreach (var unitData in unitSaveData)
        {
            var instance = Instantiate(unitIconPrefab,parent);
            instance.Init(unitData);
        }
    }

    public void SetSpawnEndAction(Action action)
    {
        spawnEndBtn.onClick.AddListener(()=>action?.Invoke());
    }

    public void SetPos(Vector2 pos,bool tween = false,float duration = 0.5f,Ease ease = Ease.OutQuad)
    {
        backGround.DOComplete();
        if (!tween)
        {
            backGround.anchoredPosition = pos;
        }
        else
        {
            backGround.DOAnchorPos(pos, duration).SetEase(ease);
        }
    }
    
}