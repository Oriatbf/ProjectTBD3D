using UnityEngine;

public class UiInstance : MonoBehaviour
{
    [SerializeField] private HealthContent healthContent;
    private PoolManager poolManager;
 
    private void Awake()
    {
       poolManager = GetComponent<PoolManager>(); ;
       if(healthContent != null) poolManager.CreatePool(healthContent);
    }
}
