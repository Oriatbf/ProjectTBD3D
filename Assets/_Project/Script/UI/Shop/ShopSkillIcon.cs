using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSkillIcon : Icon,IBuyable
{
    public bool isBuyed { get; set; }
    public int value { get; set; }
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI priceTxt;

    public void SetBtn()
    {
        isBuyed = false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(BtnAction);
        
        value = ShopHelper.ReturnValue(_skillBase.GetData().Rarity);
        priceTxt.text = $"{value}G";
    }

    private void BtnAction()
    {
        if (!ShopHelper.Buy(value) && !isBuyed) return;
        isBuyed = true;
        SetFrameColor(Color.red,true);
        var shopCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<ShopCanvas>("ShopCanvas");
        shopCanvas.ChangeState(false,true);
        ApplicationManager.Inst.GetModule<SkillChangeController>()
            .SetLootSkill(_skillBase,()=>shopCanvas.ChangeState(true,true,true));

    }

  
}
