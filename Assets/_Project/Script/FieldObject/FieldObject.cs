using _Project.Pooling;
using UnityEngine;

namespace _Project.Script.FieldObject
{
    public class FieldObject : MonoBehaviour,IPoolable
    {
        public void OnSpawnFromPool()
        {
            throw new System.NotImplementedException();
        }

        public void OnReturnToPool()
        {
            throw new System.NotImplementedException();
        }

        public void OnDestroyFromPool()
        {
            throw new System.NotImplementedException();
        }
    }
}