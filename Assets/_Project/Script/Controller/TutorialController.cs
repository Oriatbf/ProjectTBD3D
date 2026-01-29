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
        public Transform transform;
        public Vector2 offset = Vector2.zero;
        public Vector2 highLightSize;
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
            if (Input.GetKeyDown(KeyCode.L))
            {
                TBDLogger.CommandLog(KeyCode.L,this);
                StartTutorial();
            }
        }

        public void StartTutorial()
        {
            if(_tutorialInfos.Count == 0)Debug.LogError("튜토리얼이 없음");
            if (curIndex >= _tutorialInfos.Count)
            {
                _tutorialCanvas.ChangeState(false,true);
                return;
            }
            Debug.Log("튜토리얼 시작");
            _tutorialCanvas.ChangeState(true, true,true);
            var curTutorial = _tutorialInfos[curIndex++];
            if(curTutorial.transformType == TransformType.Rect)
                _tutorialCanvas.MoveHighlight(curTutorial.highLightRect,curTutorial.offset);
            else
                _tutorialCanvas.MoveHighlight(curTutorial.transform,curTutorial.offset);
            _tutorialCanvas.SetHighlightAction(curTutorial.btnAction);
            _tutorialCanvas.SetText(curTutorial.informationTxt);
            
        }
        

        public void SetTutorial(TutorialInfo tutorialInfo)
        {
            tutorialInfo.btnAction += StartTutorial;
            _tutorialInfos.Add(tutorialInfo);
            SortTutorial();
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