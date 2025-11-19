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
    
    private readonly string iconPath =  "Assets/_Project/Art/Icons/UsingIcon/";

    public async void Init(string iconName, string info)
    {
        icon.sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath + iconName + ".png").ToUniTask();
        nameTxt.text = info;
    }
    
    public async void Init(SkillData.SkillBase skill)    
    {
        icon.sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath + skill.GetData().SpriteName + ".png").ToUniTask();
        nameTxt.text = skill.GetData().Infor;
    }
}
