using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TopInfoController : BaseController
{
    TopInfoCanvas topInfoCanvas;
    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    private  void SetCanvas()
    {
        topInfoCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<TopInfoCanvas>("TopInfoCanvas");
    }

    public void AddGold(int value)
    {
        topInfoCanvas.AddGold(value);
    }
    
    public TopInfoCanvas GetTopInfoCanvas() => topInfoCanvas;
    
}
