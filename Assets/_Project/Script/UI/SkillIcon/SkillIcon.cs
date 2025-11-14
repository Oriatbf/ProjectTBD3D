using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    protected SkillStackInfo skillStackInfo;
    private readonly string iconPath = "Assets/_Project/Art/Icons/UsingIcon/";
    

    public virtual void Init(SkillStackInfo skillStackInfo)
    {
        this.skillStackInfo = skillStackInfo;
        var skill = skillStackInfo.skill;
        if (icon != null) SetSprite(skill.GetData().SpriteName);
    }

    private async void SetSprite(string spriteName)
    {
        var sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath+spriteName+".png").Task;
        icon.sprite = sprite;
    }
    public SkillStackInfo GetSkillStackInfo() => skillStackInfo;

}
