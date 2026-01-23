using _Project.Script.UI.Canvas;
using UnityEngine;

namespace _Project.Script.Controller
{
    public class TitleFlowController : BaseController
    {
        TitleCanvas _titleCanvas;
        TitleMenuCanvas _titleMenuCanvas;
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
            _titleCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<TitleCanvas>("TitleCanvas");
            _titleMenuCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<TitleMenuCanvas>("TitleMenuCanvas");
            _titleCanvas.ChangeState(true,false,true);
            _titleCanvas.StartTitle(TitleEndHandle);
        }

        private void TitleEndHandle()
        {
            _titleCanvas.ChangeState(false);
            _titleMenuCanvas.ChangeState(true,false,true);
        }
    }
}