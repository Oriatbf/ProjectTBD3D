using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LootIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private Button selectBtn;
    
    private readonly string iconPath =  "Assets/_Project/Art/Icons/UsingIcon/";

    public async void Init(string iconName, string info,Action action)
    {
        icon.sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath + iconName + ".png").ToUniTask();
        nameTxt.text = info;
        selectBtn.onClick.AddListener(() => action?.Invoke());
    }
    
    public async void Init(SkillData.SkillBase skill,Action action)    
    {
        icon.sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath + skill.GetData().SpriteName + ".png").ToUniTask();
        nameTxt.text = skill.GetData().Infor;
        selectBtn.onClick.AddListener(() => action?.Invoke());
    }
    
    public Button GetSelectBtn()=>selectBtn;
}
