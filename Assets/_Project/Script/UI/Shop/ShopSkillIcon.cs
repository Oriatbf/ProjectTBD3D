using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSkillIcon : Icon,IBuyable
{
    public int value { get; set; }
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI priceTxt;

    public void SetBtn()
    {
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(BtnAction);
        
        value = ShopHelper.ReturnValue(skillBase.GetData().Rarity);
        priceTxt.text = $"{value}G";
    }

    private void BtnAction()
    {
        if (!ShopHelper.Buy(value)) return;
        var shopCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<ShopCanvas>("ShopCanvas");
        shopCanvas.ChangeState(false,true);
        ApplicationManager.Inst.GetModule<SkillChangeController>()
            .SetLootSkill(skillBase,()=>shopCanvas.ChangeState(true,true,true));

    }

  
}
