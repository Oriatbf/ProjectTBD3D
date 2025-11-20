using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TopInfoController : BaseController
{
    TopInfoCanvas topInfoCanvas;
    private readonly string canvasPath = "Assets/_Project/Prefab/UI/TopInfoCanvas.prefab";
    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    private async void SetCanvas()
    {
        var canvas = await Addressables.LoadAssetAsync<GameObject>(canvasPath).ToUniTask();
        var obj = GameObject.Instantiate(canvas);
        topInfoCanvas = obj.GetComponent<TopInfoCanvas>();
    }

    public void AddGold(int value)
    {
        topInfoCanvas.AddGold(value);
    }
    
}
