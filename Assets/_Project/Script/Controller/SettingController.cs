using UnityEngine;


namespace _Project.Script.Controller
{
    public class SettingController : BaseController
    {
        SettingCanvas _settingCanvas;
        public override ControllerInfo ControllerInfo { get; } = new()
        {
            ContainSceneNames = new string[] {"GamePlay","MapScene" },
            Priority = 0,
            UpdateInterval = 1,
            LateUpdateInterval = 0,
            FixedUpdateInterval = 0,
        };

        public override void OnInitialize()
        {
            base.OnInitialize();
            _settingCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<SettingCanvas>("SettingCanvas");
            
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!_settingCanvas.isShow)
                {
                    _settingCanvas.ChangeState(true,true,true);    
                }
                else
                {
                    _settingCanvas.ChangeState(false,true);
                    DataManager.Inst.JsonSave();
                }
                
            }
        }
    }
}