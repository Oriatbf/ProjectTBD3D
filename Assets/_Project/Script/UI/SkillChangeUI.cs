using SkillData;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillChangeUICanvas : BaseCanvas
{
    [FormerlySerializedAs("changeSkillIcon")] [SerializeField] private ChangeIcon changeIcon;
    
    public void Init(SkillBase skillBase)
    {
        changeIcon.Init(skillBase);
    }

    public void Show()
    {
        if (isShow) return;
        ChangeState(true,true,true);
        isShow = true;
    }

    public void Hide()
    {
        if(!isShow) return;
        ChangeState(false,true,false);
        isShow = false;
    }
}
