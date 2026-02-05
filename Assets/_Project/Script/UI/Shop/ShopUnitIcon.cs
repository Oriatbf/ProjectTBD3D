using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUnitIcon : UnitIcon,IBuyable
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
        
        value = ShopHelper.ReturnValue(_unitSaveData.rarity);
        priceTxt.text = $"{value}G";
    }

    private void BtnAction()
    {
        if(!ShopHelper.Buy(value) || isBuyed)return;
        isBuyed = true;
        SetFrameColor(Color.red,true);
        DataManager.Inst.SaveUnit(_unitSaveData);

    }

   
}
