using UnityEngine;

public class UnitController : MonoBehaviour
{
    protected SheetDataManager sheetDataManager;
    protected ApplicationManager applicationManager;

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        applicationManager = ApplicationManager.Inst;
        sheetDataManager = DIContainer.ResolveService<SheetDataManager>();
    }
}
