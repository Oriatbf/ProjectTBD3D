using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerSpawnController : BaseController
{
    private string canvasPath = "Assets/_Project/Prefab/UI/ChracterInfoCanvas/PlayerSpawnCanvas.prefab";
    private Tile lastTile;
    private int maxcount = 2;
    private int currentcount = 0;

    public virtual void OnInitialize()
    {
        SetCanvas();
    }

    public async void SetCanvas()
    {
        var canvas = await Addressables.LoadAssetAsync<GameObject>(canvasPath).ToUniTask();
        GameObject.Instantiate(canvas);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && maxcount > currentcount)
        {
            if (hit.transform.TryGetComponent(out Tile tile))
            {
                if(Input.GetMouseButtonDown(0))
                {
                   FactoryManager.Inst.PlayerSpawn(0,tile);
                   currentcount++;
                   tile.UnTarget();
                }

                if (lastTile == tile) return;
                if (lastTile != null)lastTile.UnTarget();
                tile.Target();
                lastTile = tile;
            }
        }

        if (maxcount < currentcount && lastTile != null)
        {
            lastTile.UnTarget();
            lastTile = null;
        }

    }
}