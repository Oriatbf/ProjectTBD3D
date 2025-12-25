using UnityEngine;
using UnityEngine.UI;

public class ShopController : BaseController
{
    private Button exitBtn;
    private SkillZone skillZone;
    private bool isFirstOpen = false;
    ShopCanvas shopCanvas;

    
    public override void OnInitialize()
    {
        base.OnInitialize();
        var applicationManager  = ApplicationManager.Inst;
        var canvas = applicationManager.GetModule<CanvasController>().GetCanvas("ShopCanvas");
        if(canvas != null) shopCanvas = canvas.GetComponent<ShopCanvas>();
        // exitBtn.onClick.AddListener(()=>Hide());
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
