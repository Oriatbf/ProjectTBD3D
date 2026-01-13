using System;
using System.Collections.Generic;
using _Project.Resources.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Pooling
{
    /// <summary>
    /// 게임 전체의 오브젝트 풀을 관리하는 컨트롤러
    /// BaseController를 상속하는 버전
    /// </summary>
    public class PoolController : BaseController
    {
        // Type 기반 풀 저장소
        private readonly Dictionary<Type, object> typePools = new Dictionary<Type, object>();
        
        // Key(string) 기반 풀 저장소 - PoolingLoader와 호환
        private readonly Dictionary<string, ObjectPool<Component>> keyPools = new Dictionary<string, ObjectPool<Component>>();
        
        // 각 풀의 부모 Transform
        private readonly Dictionary<string, Transform> poolParents = new Dictionary<string, Transform>();
        
        private Transform poolRootParent;
        private PoolingLoader poolingLoader;

        public override void OnInitialize()
        {
            base.OnInitialize();
            
            poolRootParent = new GameObject("PoolRoot").transform;
            LoadPoolingData();
        }

        /// <summary>
        /// PoolingLoader 데이터 로드 및 풀 생성
        /// </summary>
        private void LoadPoolingData()
        {
            poolingLoader = UnityEngine.Resources.Load<PoolingLoader>("Loader/PoolingLoader");
            
            if (poolingLoader == null)
            {
                Debug.LogWarning("PoolingLoader not found at Resources/Loader/PoolingLoader");
                return;
            }

            foreach (var poolInfo in poolingLoader.PoolList)
            {
                if (poolInfo.Prefab == null)
                {
                    Debug.LogWarning($"Prefab is null for key: {poolInfo.Key}");
                    continue;
                }

                CreatePoolFromLoader(poolInfo);
            }

            Debug.Log($"PoolController initialized with {keyPools.Count} pools from PoolingLoader");
        }

        /// <summary>
        /// PoolingLoader 정보로 풀 생성
        /// </summary>
        private void CreatePoolFromLoader(PoolingLoader.PoolInfo poolInfo, int initialSize = 5, int maxSize = 50)
        {
            var key = poolInfo.Key;
            var prefab = poolInfo.Prefab;
            if (keyPools.ContainsKey(key))
            {
                Debug.LogWarning($"Pool with key '{key}' already exists");
                return;
            }

            Transform parent = GetOrCreatePoolParent(poolInfo);
            Component component = prefab.GetComponent<Transform>();
            Debug.Log(key);
            var pool = new ObjectPool<Component>(component, parent, initialSize, maxSize);
            keyPools[key] = pool;
        }

      

      
        /// <summary>
        /// Type 기반으로 오브젝트 스폰
        /// </summary>
        public T Spawn<T>() where T : Component
        {
            Type type = typeof(T);
            
            if (typePools.TryGetValue(type, out object poolObj))
            {
                ObjectPool<T> pool = (ObjectPool<T>)poolObj;
                Debug.Log(pool.Get());
                return pool.Get();
            }
            

            Debug.LogError($"Pool for type {type.Name} not found. Create pool first using CreatePool<T>()");
            return null;
        }

        /// <summary>
        /// Type 기반으로 위치와 회전 지정하여 스폰
        /// </summary>
        public T Spawn<T>(Vector3 position, Quaternion rotation) where T : Component
        {
            T obj = Spawn<T>();
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            return obj;
        }

        /// <summary>
        /// Key 기반으로 오브젝트 스폰 (PoolingLoader 사용)
        /// </summary>
        public T Spawn<T>(string key) where T : Component
        {
            if (keyPools.TryGetValue(key, out ObjectPool<Component> pool))
            {
                Component obj = pool.Get();
                if (obj == null) return null;

                if (obj.TryGetComponent(out T t)) return t;
                else return null;

                //return obj as T;
            }

            Debug.LogError($"Pool with key '{key}' not found");
            return null;
        }

        /// <summary>
        /// Key 기반으로 위치와 회전 지정하여 스폰
        /// </summary>
        public T Spawn<T>(string key, Vector3 position, Quaternion rotation) where T : Component
        {
            T obj = Spawn<T>(key);
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            return obj;
        }

        /// <summary>
        /// GameObject 스폰 (Key 기반)
        /// </summary>
        public GameObject SpawnGameObject(string key)
        {
            if (keyPools.TryGetValue(key, out ObjectPool<Component> pool))
            {
                Component obj = pool.Get();
                return obj.gameObject;
            }

            Debug.LogError($"Pool with key '{key}' not found");
            return null;
        }

        /// <summary>
        /// GameObject 스폰 (위치, 회전 지정)
        /// </summary>
        public GameObject SpawnGameObject(string key, Vector3 position, Quaternion rotation)
        {
            GameObject obj = SpawnGameObject(key);
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            return obj;
        }

        /// <summary>
        /// Type 기반 오브젝트 반환
        /// </summary>
        public void Despawn<T>(T obj) where T : Component
        {
            if (obj == null)
            {
                Debug.LogWarning("Trying to despawn null object");
                return;
            }

            Type type = typeof(T);
            
            if (typePools.TryGetValue(type, out object poolObj))
            {
                ObjectPool<T> pool = (ObjectPool<T>)poolObj;
                pool.Return(obj);
                return;
            }

            Debug.LogWarning($"Pool for type {type.Name} not found. Destroying object instead.");
            UnityEngine.Object.Destroy(obj.gameObject);
        }

        /// <summary>
        /// Key 기반 오브젝트 반환
        /// </summary>
        public void Despawn(string key, Component obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("Trying to despawn null object");
                return;
            }

            if (keyPools.TryGetValue(key, out ObjectPool<Component> pool))
            {
                pool.Return(obj);
                return;
            }

            Debug.LogWarning($"Pool with key '{key}' not found. Destroying object instead.");
            UnityEngine.Object.Destroy(obj.gameObject);
        }

       

        /// <summary>
        /// 특정 풀 가져오기 (Type 기반)
        /// </summary>
        public ObjectPool<T> GetPool<T>() where T : Component
        {
            Type type = typeof(T);
            if (typePools.TryGetValue(type, out object pool))
            {
                return (ObjectPool<T>)pool;
            }
            return null;
        }

        /// <summary>
        /// 특정 풀 가져오기 (Key 기반)
        /// </summary>
        public ObjectPool<T> GetPool<T>(string key) where T : Component
        {
            if (keyPools.TryGetValue(key, out ObjectPool<Component> pool))
            {
                return pool as ObjectPool<T>;
            }
            return null;
        }

        /// <summary>
        /// 풀 부모 Transform 가져오기 또는 생성
        /// </summary>
        private Transform GetOrCreatePoolParent(PoolingLoader.PoolInfo poolInfo)
        {
            var key = poolInfo.Key;
            if (poolParents.TryGetValue(key, out Transform parent))
            {
                return parent;
            }

            GameObject parentObj = null;
            switch (poolInfo.poolType)
            {
                case PoolType.UI:
                    parentObj = new GameObject($"Pool_{key}_Canvas");
                    var canvas = parentObj.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    var scaler = parentObj.AddComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    scaler.matchWidthOrHeight = 0.5f;

                    parentObj.AddComponent<GraphicRaycaster>();
                    break;
                case PoolType.GameObject:
                    parentObj = new GameObject($"Pool_{key}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (parentObj == null)
            {
                Debug.LogError($"parentObj is null");
                return null;
            }
            parent = parentObj.transform;
            parent.SetParent(poolRootParent);
            poolParents[key] = parent;
            
            return parent;
        }

        /// <summary>
        /// 특정 풀 클리어 (Type 기반)
        /// </summary>
        public void ClearPool<T>() where T : Component
        {
            Type type = typeof(T);
            if (typePools.TryGetValue(type, out object poolObj))
            {
                ObjectPool<T> pool = (ObjectPool<T>)poolObj;
                pool.Clear();
                typePools.Remove(type);
            }
        }

        /// <summary>
        /// 특정 풀 클리어 (Key 기반)
        /// </summary>
        public void ClearPool(string key)
        {
            if (keyPools.TryGetValue(key, out ObjectPool<Component> pool))
            {
                pool.Clear();
                keyPools.Remove(key);
            }
        }

        /// <summary>
        /// 모든 풀 클리어
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in typePools.Values)
            {
                if (pool is ObjectPool<Component> componentPool)
                {
                    componentPool.Clear();
                }
            }
            typePools.Clear();

            foreach (var pool in keyPools.Values)
            {
                pool.Clear();
            }
            keyPools.Clear();
        }
    }
}