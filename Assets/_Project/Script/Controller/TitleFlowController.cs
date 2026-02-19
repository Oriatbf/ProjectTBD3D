using _Project.Script.UI.Canvas;
using UnityEngine;

namespace _Project.Script.Controller
{
    public class TitleFlowController : BaseController
    {
        TitleCanvas _titleCanvas;
        TitleMenuCanvas _titleMenuCanvas;
        MainCharacterSelectCanvas _mainCharacterSelectCanvas;
        public override ControllerInfo ControllerInfo { get; } = new()
        {
            ContainSceneNames = new string[] {"Title" },
            Priority = 0,
            UpdateInterval = 1,
            LateUpdateInterval = 1,
            FixedUpdateInterval = 1,
        };

        public override void OnInitialize()
        {
            base.OnInitialize();
            _titleCanvas = ApplicationManager.Inst
                .GetModule<CanvasController>().GetCanvas<TitleCanvas>("TitleCanvas");
            _titleMenuCanvas = ApplicationManager.Inst
                .GetModule<CanvasController>().GetCanvas<TitleMenuCanvas>("TitleMenuCanvas");
            _mainCharacterSelectCanvas = ApplicationManager.Inst
                .GetModule<CanvasController>().GetCanvas<MainCharacterSelectCanvas>("MainCharacterSelectCanvas");
            _titleCanvas.ChangeState(true,false,true);
            _titleCanvas.StartTitle(TitleEndHandle);
            _titleMenuCanvas.SetNewGameHandle(MainCharacterSelectHandle);
        }

        public void TitleEndHandle()
        {
            _titleCanvas.ChangeState(false,true);
            _titleMenuCanvas.ChangeState(true,false,true);
            _mainCharacterSelectCanvas.ChangeState(false,true,false);
        }

        public void MainCharacterSelectHandle()
        {
            _titleCanvas.ChangeState(false);
            _titleMenuCanvas.ChangeState(false,true);
            _mainCharacterSelectCanvas.ChangeState(true,true,true);
        }
    }
}