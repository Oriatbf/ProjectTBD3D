using System;
using _Project.Script.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialCanvas : BaseCanvas
{
    [SerializeField] private RectTransform highlightRect;
    [SerializeField] private Image dimmedImage;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button entireBtn;

    private Material _runtimeMat;
    private Button highlightBtn;
    private TutorialInfo _curTutorialInfo;

    protected override void Awake()
    {
        base.Awake();
        if (dimmedImage != null)
        {
            _runtimeMat = Instantiate(dimmedImage.material);
            dimmedImage.material = _runtimeMat;
        }

        if (highlightRect != null)
        {
            highlightBtn = highlightRect.GetComponent<Button>();
        }
    }

    void Update()
    {
        ChangeHighlightSize();

        if (_curTutorialInfo != null)
        {
            if (_curTutorialInfo.isKeyAction)
            {
                if(Input.GetKeyDown(_curTutorialInfo.keyCode))
                    _curTutorialInfo.btnAction?.Invoke();
            }
        }
    }

    public void TutorialInfoInit(TutorialInfo tutorialInfo)
    {
        _curTutorialInfo = tutorialInfo;
        highlightBtn.enabled = tutorialInfo.btnRay;
        MoveHighlight(tutorialInfo);
        SetHighlightUI(tutorialInfo);
    }
    
    
    
    private void MoveHighlight(TutorialInfo tutorialInfo)
    {
        var targetTrans = tutorialInfo.highlightTrans;
        var targetRect = tutorialInfo.highLightRect;
        var targetPos = tutorialInfo.highlightPos;
        var highlightOffset = tutorialInfo.highlightOffset;
        var textOffset = tutorialInfo.textOffset;
        
        Vector2 screenPos = Vector2.zero;
        Vector2 localPos = Vector2.zero;
        
        entireBtn.enabled = tutorialInfo.entireRay;
        
        if (tutorialInfo.transformType == TransformType.Rect)
        {
            // 타겟의 월드 위치를 스크린 포인트로 변환
            screenPos = RectTransformUtility.WorldToScreenPoint(null, targetRect.position);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                highlightRect.parent as RectTransform, 
                screenPos, 
                null, 
                out localPos
            );
            localPos += GetPivotOffset(targetRect);
        }
        else if(tutorialInfo.transformType == TransformType.Transform)
        { 
            screenPos = Camera.main.WorldToScreenPoint(targetTrans.position);
            // 스크린 포인트를 하이라이트 부모 rect의 로컬 위치로 변환
        
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                highlightRect.parent as RectTransform, 
                screenPos, 
                null, 
                out localPos
            );     
        }
        else
        {
            localPos = targetPos;

        }

        // 앵커드 포지션 설정 (UI 요소는 anchoredPosition 사용)
        highlightRect.anchoredPosition = localPos+highlightOffset;
        infoText.rectTransform.anchoredPosition = localPos + textOffset;
    }
    
    private Vector2 GetPivotOffset(RectTransform rect)
    {
        return new Vector2(
            (0.5f - rect.pivot.x) * rect.rect.width,
            (0.5f - rect.pivot.y) * rect.rect.height
        );
    }

    private void SetHighlightUI(TutorialInfo tutorialInfo)
    {
        highlightRect.sizeDelta = tutorialInfo.highLightSize;
        highlightBtn.onClick.RemoveAllListeners();
        highlightBtn.onClick.AddListener(tutorialInfo.btnAction.Invoke);
        if (tutorialInfo.entireRay)
        {
            entireBtn.onClick.RemoveAllListeners();
            entireBtn.onClick.AddListener(tutorialInfo.btnAction.Invoke);
        }
        infoText.text = tutorialInfo.informationTxt;
    }

    private void ChangeHighlightSize()
    {
        if (highlightRect == null || _runtimeMat == null)
            return;

        Vector3[] corners = new Vector3[4];
        highlightRect.GetWorldCorners(corners);

        Vector2 min = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
        Vector2 max = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

        Vector2 center = (min + max) * 0.5f;
        Vector2 size = max - min;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        _runtimeMat.SetVector("_HoleCenter", center / screenSize);
        _runtimeMat.SetVector("_HoleSize", size / screenSize);
    }
}