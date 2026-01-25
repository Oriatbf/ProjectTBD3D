using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSkillIcon : Icon
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI priceTxt;
    

    public void SetBtn()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(BtnAction);
        priceTxt.text = "100" + "G";
    }

    private void BtnAction()
    {
        var curGold = DataManager.Inst.GetGold();
        if (curGold < 100) return;
        var shopCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<ShopCanvas>("ShopCanvas");
        shopCanvas.ChangeState(false,true);
        ApplicationManager.Inst.GetModule<TopInfoController>().AddGold(-100);
        ApplicationManager.Inst.GetModule<SkillChangeController>()
            .SetLootSkill(skillBase,()=>shopCanvas.ChangeState(true,true,true));

    }
}
