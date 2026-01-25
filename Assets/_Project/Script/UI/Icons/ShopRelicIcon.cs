using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopRelicIcon : RelicIcon
{
   [SerializeField] private Button butBtn;
   [SerializeField] private TextMeshProUGUI priceTxt;
   
   public void SetBtn()
   {
      butBtn.onClick.RemoveAllListeners();
      butBtn.onClick.AddListener(BtnAction);
      priceTxt.text = "100" + "G";
   }

   private void BtnAction()
   {
      var curGold = DataManager.Inst.GetGold();
      if (curGold < 100)
      {
         Debug.Log("골드가 부족합니다");
         return;
      }
      ApplicationManager.Inst.GetModule<TopInfoController>().AddGold(-curGold);
      DataManager.Inst.SaveRelic(_relicBase.GetData().ID);

   }
}
