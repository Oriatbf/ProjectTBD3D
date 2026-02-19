using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillChangeController : BaseController
{

    SkillChangeInventoryCanvas _skillChangeInventoryCanvas;
    Action hideHandle;
    
    public override ControllerInfo ControllerInfo { get; } = new()
    {
        ContainSceneNames = new string[] {"GamePlay" },
        Priority = 0,
        UpdateInterval = 0,
        LateUpdateInterval = 0,
        FixedUpdateInterval = 0,
    };
    
    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
     
    }

    private  void SetCanvas()
    {
        var canvasController = ApplicationManager.Inst.GetModule<CanvasController>();
        _skillChangeInventoryCanvas =
            canvasController.GetCanvas<SkillChangeInventoryCanvas>("SkillChangeInventoryCanvas");
        _skillChangeInventoryCanvas.SetCloseAction(Hide);
    }

    private void Hide()
    {
        _skillChangeInventoryCanvas.Hide();
        hideHandle?.Invoke();
    }
    
    
    /// <summary>
    /// 스킬 변경 UI 띄우기
    /// </summary>
    public void SetLootSkill(SkillBase skillBase,Action hideAction = null)
    {
        _skillChangeInventoryCanvas.Show();
        _skillChangeInventoryCanvas.InitChangeSkill(skillBase);
        hideHandle = hideAction;
    }

  
}
