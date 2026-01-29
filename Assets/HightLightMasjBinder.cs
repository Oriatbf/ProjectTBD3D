using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialCanvas : BaseCanvas
{
    [SerializeField] private RectTransform highlightRect;
    [SerializeField] private Image dimmedImage;
    [SerializeField] private TextMeshProUGUI infoText;

    private Material _runtimeMat;
    private Button highlightBtn;

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
    }

    public void MoveHighlight(RectTransform target,Vector2 offset)
    {
        
        // 타겟의 월드 위치를 스크린 포인트로 변환
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);

        // 스크린 포인트를 하이라이트 부모 rect의 로컬 위치로 변환
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            highlightRect.parent as RectTransform, 
            screenPos, 
            null, 
            out localPos
        );

        // 앵커드 포지션 설정 (UI 요소는 anchoredPosition 사용)
        highlightRect.anchoredPosition = localPos + offset;
        infoText.rectTransform.anchoredPosition = localPos + new Vector2(100, 100);
    }

    public void MoveHighlight(Transform target,Vector2 offset)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
        // 스크린 포인트를 하이라이트 부모 rect의 로컬 위치로 변환
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            highlightRect.parent as RectTransform, 
            screenPos, 
            null, 
            out localPos
        );

        // 앵커드 포지션 설정 (UI 요소는 anchoredPosition 사용)
        highlightRect.anchoredPosition = localPos+offset;
        infoText.rectTransform.anchoredPosition = localPos + new Vector2(100, 100);
    }

    public void SetHighlightAction(Action action)
    {
        highlightBtn.onClick.RemoveAllListeners();
        highlightBtn.onClick.AddListener(action.Invoke);
    }

    public void SetText(string text)
    {
        infoText.text = text;
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