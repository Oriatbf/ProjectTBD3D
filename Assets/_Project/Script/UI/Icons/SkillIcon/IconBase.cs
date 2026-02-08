using System;
using _Project.Script.Controller;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public enum IconState
{
    Selected,Blocked,None
}

public class IconBase : MonoBehaviour
{
    [SerializeField] protected Image icon;
    [SerializeField] protected Image frame;
    protected IconState _curIconState = IconState.None;
    private readonly string iconPath = "Assets/_Project/Art/Icons/UsingIcon/";
    
    protected async UniTask SetSprite(string spriteName)
    {
        var sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath+spriteName+".png").Task;
        icon.sprite = sprite;
        AlphaIcon(1);
    }

    protected void AlphaIcon(float alpha,bool isDotween = false,float duration = 0.25f)
    {
        icon.DOKill();
        if (isDotween) icon.DOFade(alpha, duration);
        else icon.DOFade(alpha, 0);
    }

    public void SetFrameColor(IconState iconState,bool isDotween = false,bool isSound = true,float duration = 0.25f)
    {
        frame.DOKill();
        if (_curIconState == iconState) return;
        _curIconState = iconState;
        if (ApplicationManager.Inst.GetModule<GameFlowController>().GetCurNodeType() != NodeType.Tutorial && isSound) 
            ApplicationManager.Inst.GetModule<AudioController>().PlayAudio("Btn");
        Color targetColor =Color.black;
        
        switch (iconState)
        {
            case IconState.Selected:
                targetColor = Color.green;
                break;
            case IconState.Blocked:
                targetColor = Color.red;
                break;
            case IconState.None:
                targetColor = Color.white;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(iconState), iconState, null);
        }
        
        if (isDotween)
        {
            frame.DOColor(targetColor, duration);
        }
        else
        {
            frame.color = targetColor;
        }
    }

    protected virtual void Reset()
    {
        icon.sprite = null;
    }

}


