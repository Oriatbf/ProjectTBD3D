using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Project.Script.UI.Canvas
{
    public class TitleCanvas : BaseCanvas
    {
        [SerializeField] private TextMeshProUGUI titleText,clickTxt;
        
        private bool _titleAnimating = true;
        private Action titleEndHandle;

        public void StartTitle(Action titleEndAction)
        {
            titleEndHandle = titleEndAction;
            titleText.DOComplete();
            clickTxt.DOComplete();
            titleText.color = new Color(1, 1, 1, 0);
            clickTxt.color  = new Color(1, 1, 1, 0);

            Sequence seq = DOTween.Sequence()
                .SetUpdate(true);

            seq.Append(titleText.DOFade(1f, 0.5f));
            seq.Join(clickTxt.DOFade(1f, 0.5f));
            seq.AppendCallback(()=>_titleAnimating = false);
            seq.Play();
            
            clickTxt.DOFade(0.3f, 0.8f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && isShow)
            {
                if (_titleAnimating)
                {
                    titleText.DOComplete();
                    _titleAnimating = true;
                }
                else
                {
                    titleEndHandle();
                }
            }
          
            
        }
    }
}