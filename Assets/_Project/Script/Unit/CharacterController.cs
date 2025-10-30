using UnityEngine;

public class CharacterController : MonoBehaviour
{
    protected SheetDataManager sheetDataManager;
    protected ApplicationManager applicationManager;

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        applicationManager = DIContainer.ResolveService<ApplicationManager>();
        sheetDataManager = DIContainer.ResolveService<SheetDataManager>();
    }
}
