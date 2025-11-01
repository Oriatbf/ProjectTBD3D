using UnityEngine;
using UnityEngine.AddressableAssets;

public class CharacterInfoController : BaseController
{
    private string canvasPath = "Assets/_Project/Prefab/UI/CharacterInfoCanvas";
    public override void OnIntialize()
    {
        base.OnIntialize();
        SetCanvas();
    }

    private async void SetCanvas()
    {
        var canvas =  await Addressables.LoadAssetAsync<Canvas>(canvasPath).Task;
        var obj = GameObject.Instantiate(canvas);
    }
    
    
}
