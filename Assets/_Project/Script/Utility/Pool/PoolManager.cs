using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<Type,object> pools = new Dictionary<Type, object>();
    private readonly Dictionary<Type,Transform>poolsParent = new Dictionary<Type, Transform>();

    public ObjectPool<T> CreatePool<T>(T prefab, int initialSize = 10, int maxSize = 100) where T : Component,IPoolable
    {
        Type type = typeof(T);

        if (pools.ContainsKey(type))  return (ObjectPool<T>)pools[type]; //todo 뭔지 모르겠움
        
        GameObject canvasObj = new GameObject("MyCanvas", typeof(Canvas));
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.transform.SetParent(transform);
        
        ObjectPool<T> pool = new ObjectPool<T>(prefab, canvasObj.transform, initialSize, maxSize);
        pools[type] = pool;
        return pool;
    }

    public T Spawn<T>() where T : Component, IPoolable
    {
        Type type = typeof(T);
        if (pools.TryGetValue(type, out object poolObj))
        {
            ObjectPool<T> pool = (ObjectPool<T>)poolObj;
            return pool.Get();
        }
        else
        {
            Debug.LogError("스폰 하지 못함");
        }
        return null;
    }
    
    public void Despawn<T>(T obj) where T : Component, IPoolable
    {
        if (obj == null) return;

        Type type = typeof(T);
        if (pools.TryGetValue(type, out object poolObj))
        {
            ObjectPool<T> pool = (ObjectPool<T>)poolObj;
            pool.Return(obj);
        }
        else
        {
            Debug.LogWarning($"Pool for type {type.Name} not found! Destroying object.");
            Destroy(obj.gameObject);
        }
    }

    
}
