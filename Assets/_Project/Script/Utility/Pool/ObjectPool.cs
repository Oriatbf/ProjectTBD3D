using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    /// <summary>풀에서 가져왔을 때 호출</summary>
    void OnSpawnFromPool();
        
    /// <summary>풀로 반환될 때 호출</summary>
    void OnReturnToPool();
        
    /// <summary>오브젝트가 파괴될 때 호출</summary>
    void OnDestroyFromPool();
}

public class ObjectPool<T> where T: Component, IPoolable
{
    private readonly Queue<T> pool = new Queue<T>();
    private readonly T prefab;
    private readonly Transform parent;
    private readonly int initialSize;
    private readonly int maxSize;
    
    public int Count => pool.Count;
    public int MaxSize => maxSize;

    public ObjectPool(T prefab, Transform parent, int initialSize, int maxSize)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.initialSize = initialSize;
        this.maxSize = maxSize;
        
        Initialize();
    }

    private void Initialize()
    {
        for(int i = 0; i < initialSize; i++)CreateNewObject();
    }

    private T CreateNewObject()
    {
        T obj = UnityEngine.Object.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    public T Get()
    {
        T obj;
        if(pool.Count > 0)obj = pool.Dequeue();
        else
        {
            obj = CreateNewObject();
            pool.Dequeue();
        }
        obj.gameObject.SetActive(true);
        obj.OnSpawnFromPool();
        return obj;
    }

    public void Return(T obj)
    {
        if (obj == null) return;

        obj.OnReturnToPool();
        obj.gameObject.SetActive(false);
        if (pool.Count >= maxSize)
        {
            obj.OnDestroyFromPool();
            UnityEngine.Object.Destroy(obj.gameObject);
        }
        else
        {
            pool.Enqueue(obj);
        }
    }

}
