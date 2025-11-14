using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LootController : BaseController
{
    private string canvasPath = "";
    private string lootPath = "";
    LootCanvas lootCanvas;
    LootSkillIcon lootSkillIcon;

    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    public void InitEnemyArrange(EnemyArrangeSO enemyArrangeSo)
    {
        foreach (var lootData in enemyArrangeSo.lootDatas)
        {
            
        }
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
