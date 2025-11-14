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
    
    private CharacterHead _characterHeadPrefab;
    private readonly string characterHeadPath = "Assets/_Project/Prefab/UI/ChracterInfoCanvas/CharacterHead.prefab";



    public async void  Init(List<UnitSaveData> unitSaveData)
    {
         var obj = await Addressables.LoadAssetAsync<GameObject>(characterHeadPath).ToUniTask();
         _characterHeadPrefab = obj.GetComponent<CharacterHead>();
        foreach (var unitData in unitSaveData)
        {
            var instance = Instantiate(_characterHeadPrefab,parent);
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