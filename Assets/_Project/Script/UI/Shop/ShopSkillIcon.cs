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
        ApplicationManager.Inst.GetModule<TopInfoController>().AddGold(-curGold);
        ApplicationManager.Inst.GetModule<SkillChangeController>().SetLootSkill(skillBase);

    }
}
