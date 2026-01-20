using System;
using System.Collections.Generic;
using _Project.Resources.Loader;
using Cysharp.Threading.Tasks;
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
        
        public T Spawn<T>(string key, Vector3 position) where T : Component
        {
            T obj = Spawn<T>(key);
            if (obj != null)
            {
                obj.transform.position = position;
            }
            return obj;
        }

        
        public void ReturnToPool(string key,Transform obj)
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
        }
        
        public async void ReturnToPoolDelay(string key,Transform obj,float delay = 0.2f)
        {
            await UniTask.WaitForSeconds(delay);
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
                    canvas.sortingOrder = poolInfo.canvasOrder;
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

            foreach (var pool in keyPools.Values)
            {
                pool.Clear();
            }
            keyPools.Clear();
        }
    }
}