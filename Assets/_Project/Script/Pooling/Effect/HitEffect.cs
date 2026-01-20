using System;
using _Project.Pooling;
using UnityEngine;

public class HitEffect : EffectBase
{
    private void Update()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 1f && !_animator.IsInTransition(0))
        {
            ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool("HitEffect",transform);
        }
    }
}
