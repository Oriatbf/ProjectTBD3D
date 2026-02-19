using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopRelicIcon : RelicIcon,IBuyable
{
   public bool isBuyed { get; set; }
   public int value { get; set; }
   [SerializeField] private Button butBtn;
   [SerializeField] private TextMeshProUGUI priceTxt;
   
   public void SetBtn()
   {
      isBuyed = false;
      butBtn.onClick.RemoveAllListeners();
      butBtn.onClick.AddListener(BtnAction);
     
      value = ShopHelper.ReturnValue(_relicBase.GetData().Rarity);
      priceTxt.text = $"{value}G";
   }

   private void BtnAction()
   {
      if (isBuyed) return;
      if(!ShopHelper.Buy(value))return;
      SetFrameColor(IconState.Blocked,true,false);
      DataManager.Inst.SaveRelic(_relicBase.GetData().ID);
      isBuyed = true;
      ApplicationManager.Inst.GetModule<TopInfoController>().GetTopInfoCanvas().RefreshRelic();

   }
   
}
