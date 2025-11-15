using UnityEngine;

public class UiInstance : MonoBehaviour
{
    [SerializeField] private HealthContent healthContent;
    [SerializeField] private StageContent stageContent;
    private PoolManager poolManager;
 
    private void Awake()
    {
       poolManager = GetComponent<PoolManager>(); ;
       if(healthContent != null) poolManager.CreatePool(healthContent);
       if(stageContent != null)poolManager.CreatePool(stageContent);
    }
}
