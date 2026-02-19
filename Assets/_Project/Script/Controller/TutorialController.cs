using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Script.Controller
{

    public enum TransformType
    {
        Rect,Transform,Position
    }
    public class TutorialInfo
    {
        public int order;
        public string informationTxt;
        public TransformType transformType;
        public RectTransform highLightRect;
        public Transform highlightTrans;
        public Vector2 highlightPos;
        public Vector2 highlightOffset = Vector2.zero;
        public Vector2 textOffset = Vector2.zero;
        public Vector2 highLightSize = new Vector2(100,100);
        public Action btnAction;
        public bool entireRay = false;
        public bool btnRay = true;
        public bool isKeyAction = false;
        public KeyCode keyCode = KeyCode.None;
        public string tutorialKey = "Battle";

    }
    public class TutorialController : BaseController
    {
        private TutorialCanvas _tutorialCanvas;
        
        private Dictionary<string, List<TutorialInfo>> _tutorialDict= new Dictionary<string, List<TutorialInfo>>();
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

        public void StartTutorial(string key)
        {
            var tutorialInfos = _tutorialDict[key];
            if(tutorialInfos.Count == 0)Debug.LogError("튜토리얼이 없음");
            if (curIndex >= tutorialInfos.Count)
            {
                _tutorialCanvas.ChangeState(false,true);
                if(key == "Loot") TutorialEnd(true);
                else TutorialEnd();
                return;
            }
            Debug.Log("튜토리얼 시작");
            _tutorialCanvas.ChangeState(true, true,true);
            var curTutorial = tutorialInfos[curIndex++];
            _tutorialCanvas.TutorialInfoInit(curTutorial);
        }

        public void SetTutorial(TutorialInfo tutorialInfo)
        {
            if(!_tutorialDict.ContainsKey(tutorialInfo.tutorialKey))
                _tutorialDict.Add(tutorialInfo.tutorialKey, new List<TutorialInfo>());
            _tutorialDict[tutorialInfo.tutorialKey].Add(tutorialInfo);
            tutorialInfo.btnAction += ()=>StartTutorial(tutorialInfo.tutorialKey);
            SortTutorial(tutorialInfo.tutorialKey);
        }

        private void TutorialEnd(bool endStage = false)
        {
            Debug.Log("튜토리얼 끝");
            if (endStage)
            {
                var mainCharacterId = DataManager.Inst.Data.mainCharacterID;
                DataManager.Inst.SetMainCharcter(1);    
            }
            curIndex = 0;
            
        }
        public void HideTutorial()
        {
            _tutorialCanvas.ChangeState(false, true);
        }

        private void SortTutorial(string key)
        {
            var tutorialInfos = _tutorialDict[key];
            tutorialInfos.Sort((x, y) => x.order - y.order);
        }
        
    }
}