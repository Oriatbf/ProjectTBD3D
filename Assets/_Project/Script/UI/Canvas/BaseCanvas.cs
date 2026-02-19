using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Canvas))]
public class BaseCanvas : MonoBehaviour
{
    [Tooltip("포지션을 움직일 부모 객체")]
    [SerializeField] private RectTransform parent;
    protected CanvasGroup canvasGroup;
    protected Canvas canvas;
    public bool isShow = false;

    protected virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null )Debug.LogError("canvasGroup is null");
        if(canvas == null )Debug.LogError("canvas is null");
        ForceChangeState(false);
    }

    private void ForceChangeState(bool isShow)
    {
        if(isShow)Show(false,true,0);
        else if(!isShow ) Hide(false,false,0);
        this.isShow = isShow;
    }

    public void ChangeState(bool isShow,bool isDotween = false,bool isRaycast = false,float tweenDuration =0.25f)
    {
        if(isShow && !this.isShow)Show(isDotween,isRaycast,tweenDuration);
        else if(!isShow && this.isShow) Hide(isDotween,isRaycast,tweenDuration);
    }

    public void SetPos(Vector2 pos, bool tween = false, float duration = 0.5f, Ease ease = Ease.OutQuad)
    {
        parent.DOComplete();
        if (!tween)
        {
            parent.anchoredPosition = pos;
        }
        else
        {
            parent.DOAnchorPos(pos, duration).SetEase(ease).SetUpdate(true);
        }
    }

    protected virtual void Show(bool isDotween,bool isRaycast,float tweenDuration)
    {
        canvasGroup.DOKill();
        isShow = true;
        if (isDotween)
        {
            canvasGroup.DOFade(1,tweenDuration).SetUpdate(true);
        }
        else
        {
            canvasGroup.alpha = 1;   
        }
        canvasGroup.blocksRaycasts = isRaycast;
    }

    private void Hide(bool isDotween, bool isRaycast, float tweenDuration)
    {
        canvasGroup.DOKill();
        isShow = false;
        if (isDotween)
        {
            canvasGroup.DOFade(0,tweenDuration).SetUpdate(true);
        }
        else
        {
            canvasGroup.alpha = 0;   
        }
        canvasGroup.blocksRaycasts = isRaycast;
    }
    
    public RectTransform GetParent() => parent;
}
