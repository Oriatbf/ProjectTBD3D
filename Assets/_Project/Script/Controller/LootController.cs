using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LootController : BaseController
{
    private string canvasPath = "Assets/_Project/Prefab/UI/Loot/LootCanvas.prefab";
    LootCanvas lootCanvas;

    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    public void InitEnemyArrange(EnemyArrangeSO enemyArrangeSo)
    {
        lootCanvas.Init(enemyArrangeSo);
    }

    private async void SetCanvas()
    {
        var canvas = await Addressables.LoadAssetAsync<GameObject>(canvasPath).ToUniTask();
        var obj = GameObject.Instantiate(canvas);
        if (obj.transform.TryGetComponent<LootCanvas>(out var lootCanvas))
        {
            this.lootCanvas = lootCanvas;
        }
    }


}
