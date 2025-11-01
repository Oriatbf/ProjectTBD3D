using UnityEngine;

public class UnitController : MonoBehaviour
{
    protected UnitData.Data unitData;
    protected SheetDataManager sheetDataManager;
    protected ApplicationManager applicationManager;

    protected virtual void Awake()
    {
        
    }

    public void Init(UnitData.Data unitData)
    {
        this.unitData = unitData;
    }
    protected virtual void Start()
    {
        applicationManager = ApplicationManager.Inst;
        sheetDataManager = DIContainer.ResolveService<SheetDataManager>();
    }
}
