using _Project.Pooling;
using UnityEngine;

public class CharmEffect : EffectBase
{
    
    void Update()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 1f && !_animator.IsInTransition(0))
        {
            ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool("CharmEffect",transform);
        }
    }
}
