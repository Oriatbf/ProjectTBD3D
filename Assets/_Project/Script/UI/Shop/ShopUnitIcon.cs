using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUnitIcon : UnitIcon,IBuyable
{
    public int value { get; set; }
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI priceTxt;
    

    public void SetBtn()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(BtnAction);
        
        value = ShopHelper.ReturnValue(_unitSaveData.rarity);
        priceTxt.text = $"{value}G";
    }

    private void BtnAction()
    {
        if(!ShopHelper.Buy(value))return;
        DataManager.Inst.SaveUnit(_unitSaveData);

    }

   
}
