using System;
using _Project.Pooling;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpTxt : PoolBase
{
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private Image backGround;
    

    public async UniTask SetTxt(string txt,Color color ,bool isDotween = false,bool autoHide = false ,float returnTime = 0.5f)
    {
        transform.DOKill();
        popUpText.color = color;
        popUpText.alpha = 0;
        
        popUpText.text = txt;
        if (isDotween)
        {
            await popUpText.DOFade(1, 0.25f).AsyncWaitForCompletion();
            backGround.DOFade(0.6f, 0.25f);
        }
        else popUpText.alpha = 1;

        if (!autoHide) return;
        await UniTask.WaitForSeconds(returnTime);
        await popUpText.DOFade(0, 0.25f).AsyncWaitForCompletion();
        backGround.DOFade(0, 0.25f);
        ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool("PopUpTxt",transform);
    }

    public void ReturnPool()
    {
        ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool("PopUpTxt",transform);
    }


    public override void OnSpawnFromPool()
    {
        base.OnSpawnFromPool();
        Hide();
    }

    public void Hide()
    {
        popUpText.alpha = 0;
        var color = backGround.color;
        color.a = 0;
        backGround.color = color;
    }
    
}
