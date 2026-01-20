using _Project.Pooling;
using UnityEngine;

public class PoolBase : MonoBehaviour,IPoolable
{
    public virtual void OnSpawnFromPool()
    {
        
    }

    public virtual void OnReturnToPool()
    {
        
    }

    public virtual void OnDestroyFromPool()
    {
       
    }
}
