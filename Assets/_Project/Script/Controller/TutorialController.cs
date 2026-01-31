using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Script.Controller
{

    public enum TransformType
    {
        Rect,Transform
    }
    public class TutorialInfo
    {
        public int order;
        public string informationTxt;
        public TransformType transformType;
        public RectTransform highLightRect;
        public Transform highlightTrans;
        public Vector2 highlightOffset = Vector2.zero;
        public Vector2 textOffset = Vector2.zero;
        public Vector2 highLightSize = new Vector2(100,100);
        public Action btnAction;
        
    }
    public class TutorialController : BaseController
    {
        private TutorialCanvas _tutorialCanvas;
        
        private List<TutorialInfo> _tutorialInfos = new List<TutorialInfo>();
        private int curIndex = 0;
        public override ControllerInfo ControllerInfo { get; } = new()
        {
            ContainSceneNames = new string[] {"GamePlay" },
            Priority = 0,
            UpdateInterval = 1,
            LateUpdateInterval = 0,
            FixedUpdateInterval = 0,
        };

        public override void OnInitialize()
        {
            base.OnInitialize();
            _tutorialCanvas = ApplicationManager.Inst.GetModule<CanvasController>()
                .GetCanvas<TutorialCanvas>("TutorialCanvas");
            
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public void StartTutorial()
        {
            if(_tutorialInfos.Count == 0)Debug.LogError("튜토리얼이 없음");
            if (curIndex >= _tutorialInfos.Count)
            {
                _tutorialCanvas.ChangeState(false,true);
                TutorialEnd();
                return;
            }
            Debug.Log("튜토리얼 시작");
            _tutorialCanvas.ChangeState(true, true,true);
            var curTutorial = _tutorialInfos[curIndex++];
            _tutorialCanvas.MoveHighlight(curTutorial);
            _tutorialCanvas.SetHighlightSize(curTutorial.highLightSize);
            _tutorialCanvas.SetHighlightAction(curTutorial.btnAction);
            _tutorialCanvas.SetText(curTutorial.informationTxt);
            
        }
        

        public void SetTutorial(TutorialInfo tutorialInfo)
        {
            tutorialInfo.btnAction += StartTutorial;
            _tutorialInfos.Add(tutorialInfo);
            SortTutorial();
        }

        private void TutorialEnd()
        {
            Debug.Log("튜토리얼 끝");
            var mainCharacterId = DataManager.Inst.Data.mainCharacterID;
            DataManager.Inst.SetMainCharcter(mainCharacterId);
        }
        public void HideTutorial()
        {
            _tutorialCanvas.ChangeState(false, true);
        }

        private void SortTutorial()
        {
            _tutorialInfos.Sort((x, y) => x.order - y.order);
        }
        
    }
}