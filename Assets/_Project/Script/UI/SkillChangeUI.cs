using SkillData;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillChangeUI : MonoBehaviour
{
    [SerializeField] private Panel panel;
    [FormerlySerializedAs("changeSkillIcon")] [SerializeField] private ChangeIcon changeIcon;
    private bool isShow = false;
    
    public void Init(SkillBase skillBase)
    {
        changeIcon.Init(skillBase);
    }

    public void Show()
    {
        if (isShow) return;
        panel.SetPosition(PanelStates.Show,true);
        isShow = true;
    }

    public void Hide()
    {
        if(!isShow) return;
        panel.SetPosition(PanelStates.Hide,true);
        isShow = false;
    }
}
