using UnityEngine;
using UnityEngine.UI;

public class ShopController : BaseController
{
    private bool isFirstOpen = false;
    ShopCanvas shopCanvas;

    public override ControllerInfo ControllerInfo { get; } = new()
    {
        ContainSceneNames = new string[] {"GamePlay" },
        Priority = 0,
        UpdateInterval = 1,
        LateUpdateInterval = 1,
        FixedUpdateInterval = 1,
    };

    public override void OnInitialize()
    {
        base.OnInitialize();
        var applicationManager  = ApplicationManager.Inst;
        var canvas = applicationManager.GetModule<CanvasController>().GetCanvas("ShopCanvas");
        if (canvas != null)
        {
            shopCanvas = canvas.GetComponent<ShopCanvas>();
            shopCanvas.InitExitAction(Hide);
        }
    }

    public void Show()
    {
        shopCanvas.ChangeState(true,true,true);
        if (!isFirstOpen)
        {
            isFirstOpen = true;
            RerollAll();
        }
    }
    
    public void RerollAll()
    {
        shopCanvas.Refresh();
    }

    private void Hide()
    {
        shopCanvas.ChangeState(false,true);
    }
}
