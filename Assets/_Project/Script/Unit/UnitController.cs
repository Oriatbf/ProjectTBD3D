using UnityEngine;

public class UnitController : MonoBehaviour
{
    protected UnitSaveData unitData;
    protected SheetDataManager sheetDataManager;
    protected ApplicationManager applicationManager;
    protected Tile curTile;

    protected virtual void Awake()
    {
        
    }

    public void Initialize(Tile tile)
    {
        this.curTile = tile;
    }

    public virtual void Init(UnitSaveData unitData)
    {
        this.unitData = unitData;
    }
    protected virtual void Start()
    {
        applicationManager = ApplicationManager.Inst;
        sheetDataManager = DIContainer.ResolveService<SheetDataManager>();
    }
}
