using _Project.Script.UI.Canvas;
using UnityEngine;

public class DebugController : BaseController
{
    private DebugCanvas _debugCanvas;
    private bool _isShow = false;
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
        _debugCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<DebugCanvas>("DebugCanvas");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (IsDebugTogglePressed())
        {
            if (!_isShow)
            {
                _debugCanvas.ChangeState(true,true,true);
                _isShow = true;
            }
            else
            {
                _debugCanvas.ChangeState(false,true,false);
                _isShow = false;
            }
        }
    }
    
    bool IsDebugTogglePressed()
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl);

        bool shift = Input.GetKey(KeyCode.LeftShift);
        
            

        bool tab = Input.GetKeyDown(KeyCode.Tab);

        return ctrl && shift  && tab;
    }
}
