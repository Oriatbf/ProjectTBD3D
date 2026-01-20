using UnityEngine;

public class EffectBase : PoolBase
{
    protected Animator _animator;
    public override void OnSpawnFromPool()
    {
        base.OnSpawnFromPool();
        _animator = GetComponent<Animator>();
    }

    public override void OnReturnToPool()
    {
        base.OnReturnToPool();
    }

    public override void OnDestroyFromPool()
    {
       base.OnDestroyFromPool();
    }
}
