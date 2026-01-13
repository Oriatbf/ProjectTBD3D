using System.Collections.Generic;
using UnityEngine;

namespace _Project.Pooling
{
    /// <summary>
    /// 제네릭 오브젝트 풀
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private readonly Queue<T> pool = new Queue<T>();
        private readonly HashSet<T> activeObjects = new HashSet<T>();
        private readonly T prefab;
        private readonly Transform parent;
        private readonly int initialSize;
        private readonly int maxSize;
        private readonly bool isPoolable;
        
        public int AvailableCount => pool.Count;
        public int ActiveCount => activeObjects.Count;
        public int TotalCount => AvailableCount + ActiveCount;
        public int MaxSize => maxSize;

        public ObjectPool(T prefab, Transform parent, int initialSize = 10, int maxSize = 100)
        {
            this.prefab = prefab;
            this.parent = parent;
            this.initialSize = initialSize;
            this.maxSize = maxSize;
            if (parent.TryGetComponent(out IPoolable poolable))
                this.isPoolable = true;
            else isPoolable = false;
            
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private T CreateNewObject()
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            return obj;
        }

        public T Get()
        {
            T obj;
            
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else if (TotalCount < maxSize)
            {
                obj = CreateNewObject();
            }
            else
            {
                Debug.LogWarning($"Pool for {typeof(T).Name} reached max size ({maxSize}). Reusing oldest object.");
                obj = CreateNewObject();
            }
            
            obj.gameObject.SetActive(true);
            activeObjects.Add(obj);
            
            if(obj.TryGetComponent(out IPoolable poolable))
            {
                poolable.OnSpawnFromPool();
            }
            
            return obj;
        }

        public void Return(T obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("Trying to return null object to pool");
                return;
            }

            if (!activeObjects.Contains(obj))
            {
                Debug.LogWarning($"Trying to return an object that doesn't belong to this pool: {obj.name}");
                return;
            }

            activeObjects.Remove(obj);

            if (isPoolable && obj is IPoolable poolable)
            {
                poolable.OnReturnToPool();
            }
            
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(parent);
            
            if (pool.Count >= maxSize)
            {
                if (isPoolable && obj is IPoolable destroyable)
                {
                    destroyable.OnDestroyFromPool();
                }
                Object.Destroy(obj.gameObject);
            }
            else
            {
                pool.Enqueue(obj);
            }
        }

        public void Clear()
        {
            foreach (var obj in activeObjects)
            {
                if (obj != null)
                {
                    if (isPoolable && obj is IPoolable poolable)
                    {
                        poolable.OnDestroyFromPool();
                    }
                    Object.Destroy(obj.gameObject);
                }
            }
            activeObjects.Clear();

            while (pool.Count > 0)
            {
                T obj = pool.Dequeue();
                if (obj != null)
                {
                    if (isPoolable && obj is IPoolable poolable)
                    {
                        poolable.OnDestroyFromPool();
                    }
                    Object.Destroy(obj.gameObject);
                }
            }
        }

        public void Warmup(int count)
        {
            int toCreate = count - TotalCount;
            for (int i = 0; i < toCreate && TotalCount < maxSize; i++)
            {
                CreateNewObject();
            }
        }
    }
}