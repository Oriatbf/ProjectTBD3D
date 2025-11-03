using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    private SkillBase skill;
    private readonly string iconPath = "Assets/_Project/Art/Icons/UsingIcon/Baldo.png";
    

    public void Init(SkillBase skill)
    {
        this.skill = skill;
        if (icon != null) SetSprite(skill.GetData().SpriteName);
        if(text !=null) text.text = skill.GetData().Name;
    }

    private async void SetSprite(string spriteName)
    {
        var sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath).Task;
        icon.sprite = sprite;
    }
    public SkillBase GetSkill() => skill;

}
