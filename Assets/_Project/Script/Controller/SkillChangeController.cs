using System.Linq;
using Cysharp.Threading.Tasks;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillChangeController : BaseController
{
    private readonly string skillChangeInvenCanvasPath = "Assets/_Project/Prefab/UI/Loot/SkillChangeInventory.prefab";
    private readonly string SkillChangeUICanvasPath = "Assets/_Project/Prefab/UI/Loot/SkillChangeUI.prefab";
    SkillChangeInventory skillChangeInventory;
    SkillChangeUI skillChangeUI;

    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
     
    }

    private async void SetCanvas()
    {
        var invenCanvas =  await Addressables.LoadAssetAsync<GameObject>(skillChangeInvenCanvasPath).ToUniTask();
        var invenObj = GameObject.Instantiate(invenCanvas);
        skillChangeInventory = invenObj.GetComponent<SkillChangeInventory>();
        
        var changeCanvas = await Addressables.LoadAssetAsync<GameObject>(SkillChangeUICanvasPath).ToUniTask();
        var changeObj = GameObject.Instantiate(changeCanvas);
        skillChangeUI = changeObj.GetComponent<SkillChangeUI>();
    }
    
    
    /// <summary>
    /// 스킬 변경 UI 띄우기
    /// </summary>
    public void SetLootSkill(SkillBase skillBase)
    {
        skillChangeInventory.Show();
        skillChangeUI.Init(skillBase);
        skillChangeUI.Show();
    }

  
}
