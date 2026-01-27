using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopRelicIcon : RelicIcon,IBuyable
{
   public int value { get; set; }
   [SerializeField] private Button butBtn;
   [SerializeField] private TextMeshProUGUI priceTxt;
   
   public void SetBtn()
   {
      butBtn.onClick.RemoveAllListeners();
      butBtn.onClick.AddListener(BtnAction);
     
      value = ShopHelper.ReturnValue(_relicBase.GetData().Rarity);
      priceTxt.text = $"{value}G";
   }

   private void BtnAction()
   {
      if(!ShopHelper.Buy(value))return;
      DataManager.Inst.SaveRelic(_relicBase.GetData().ID);

   }
   
}
