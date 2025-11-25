using SkillData;
using UnityEngine;

public class SkillChangeUI : MonoBehaviour
{
    [SerializeField] private Panel panel;
    [SerializeField] private ChangeSkillIcon changeSkillIcon;
    private bool isShow = false;
    
    public void Init(SkillBase skillBase)
    {
        changeSkillIcon.Init(skillBase);
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
