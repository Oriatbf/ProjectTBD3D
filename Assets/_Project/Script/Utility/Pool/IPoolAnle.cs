using UnityEngine;

namespace _Project.Pooling
{
    /// <summary>
    /// 풀링 가능한 오브젝트가 구현해야 하는 인터페이스
    /// </summary>
    public interface IPoolable
    {
        /// <summary>풀에서 가져왔을 때 호출</summary>
        void OnSpawnFromPool();
        
        /// <summary>풀로 반환될 때 호출</summary>
        void OnReturnToPool();
        
        /// <summary>오브젝트가 파괴될 때 호출 (선택적)</summary>
        void OnDestroyFromPool();
    }
}