using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Canvas))]
public class BaseCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform parent;
    protected CanvasGroup canvasGroup;
    protected Canvas canvas;
    protected bool isShow = false;

    protected virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        ChangeState(false);
    }

    public void ChangeState(bool isShow,bool isDotween = false,bool isRaycast = false,float tweenDuration =0.25f)
    {
        if(isShow)Show(isDotween,isRaycast,tweenDuration);
        else Hide(isDotween,isRaycast,tweenDuration);
        this.isShow = isShow;
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
            parent.DOAnchorPos(pos, duration).SetEase(ease);
        }
    }

    private void Show(bool isDotween,bool isRaycast,float tweenDuration)
    {
        if (isDotween)
        {
            canvasGroup.DOFade(1,tweenDuration);
        }
        else
        {
            canvasGroup.alpha = 1;   
        }
        canvasGroup.blocksRaycasts = isRaycast;
    }

    private void Hide(bool isDotween, bool isRaycast, float tweenDuration)
    {
        if (isDotween)
        {
            canvasGroup.DOFade(0,tweenDuration);
        }
        else
        {
            canvasGroup.alpha = 0;   
        }
        canvasGroup.blocksRaycasts = isRaycast;
    }
}
