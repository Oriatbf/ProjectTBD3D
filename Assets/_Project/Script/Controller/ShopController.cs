using UnityEngine;
using UnityEngine.UI;

public class ShopController : BaseController
{
    private readonly string shopCanvasPath = "";
    private Button exitBtn;
    private Panel panel;
    private SkillZone skillZone;
    private bool isFirstOpen = false;

    
    public override void OnInitialize()
    {
        base.OnInitialize();
       // exitBtn.onClick.AddListener(()=>Hide());
    }

    public void RerollALl()
    {
        skillZone.SetSkillIcon();
    }

    public void Show()
    {
        panel.SetPosition(PanelStates.Show, true);
        if (!isFirstOpen)
        {
            isFirstOpen = true;
            RerollALl();
        }
    }

    private void Hide()
    {
        panel.SetPosition(PanelStates.Hide, true);
    }
}
