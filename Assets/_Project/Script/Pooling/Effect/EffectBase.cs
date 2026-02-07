using System;
using _Project.Pooling;
using UnityEngine;

public class EffectBase : PoolBase
{
    protected Animator _animator;
    protected string keyName;
    public override void OnSpawnFromPool()
    {
        base.OnSpawnFromPool();
        _animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 1f && !_animator.IsInTransition(0) && keyName != "")
        {
            ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool(keyName,transform);
        }
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
