using System;
using _Project.Pooling;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopUpTxt : PoolBase
{
    [SerializeField] private TextMeshProUGUI popUpText;
    

    public async UniTask SetTxt(string txt,Color color ,bool isDotween = false ,float returnTime = 0.5f)
    {
        transform.DOKill();
        popUpText.color = color;
        popUpText.alpha = 0;
        
        popUpText.text = txt;
        if (isDotween)
        {
            await popUpText.DOFade(1, 0.25f).AsyncWaitForCompletion();
        }
        else popUpText.alpha = 1;
        
        await UniTask.WaitForSeconds(returnTime);
        await popUpText.DOFade(0, 0.25f).AsyncWaitForCompletion();
        ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool("PopUpTxt",transform);
    }
}
