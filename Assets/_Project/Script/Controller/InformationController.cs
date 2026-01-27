using System;
using Cysharp.Threading.Tasks;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum DataType
{
    Skill,Unit, Relic
}
public class InformationController : BaseController
{
    private SkillInfoCardCanvas _skillInfoCardCanvas;
    private UnitInfoCardCanvas _unitInfoCardCanvas;
    private RelicInfoCardCanvas relicInfoCardCanvas;

    public override ControllerInfo ControllerInfo { get; }= new()
    {
        ContainSceneNames = new string[] {"GamePlay" },
        Priority = 0,
        UpdateInterval = 1,
        LateUpdateInterval = 1,
        FixedUpdateInterval = 1,
    };

    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    private void SetCanvas()
    {
        var canvasController = ApplicationManager.Inst.GetModule<CanvasController>();
        _skillInfoCardCanvas = canvasController.GetCanvas<SkillInfoCardCanvas>("SkillInfoCardCanvas");
        _unitInfoCardCanvas = canvasController.GetCanvas<UnitInfoCardCanvas>("UnitInfoCardCanvas");
        relicInfoCardCanvas = canvasController.GetCanvas<RelicInfoCardCanvas>("RelicInfoCardCanvas");
    }

    public void InitSkillData(SkillBase skillBase,Vector3 targetPos)
    {
        _skillInfoCardCanvas.InitData(skillBase, targetPos);
    }

    public void InitUnitData(UnitSaveData unitSaveData, Vector3 targetPos)
    {
        var unitData = SheetDataManager.Inst.GetUnitData(unitSaveData.id);
        _unitInfoCardCanvas.InitData(unitData,unitSaveData, targetPos);
    }

    public void InitRelicData(RelicBase relicBase, Vector3 targetPos)
    {
        relicInfoCardCanvas.InitData(relicBase, targetPos);
    }
    
    public void Show(DataType dataType)
    {
        switch (dataType)
        {
            case DataType.Skill:
                _skillInfoCardCanvas.ChangeState(true,true,false);
                break;
            case DataType.Unit:
                _unitInfoCardCanvas.ChangeState(true,true,false);
                break;
            case DataType.Relic:
                relicInfoCardCanvas.ChangeState(true,true,false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
        }
    }

    public void Hide(DataType dataType)
    {
        switch (dataType)
        {
            case DataType.Skill:
                _skillInfoCardCanvas.ChangeState(false,true,false);
                break;
            case DataType.Unit:
                _unitInfoCardCanvas.ChangeState(false,true,false);
                break;
            case DataType.Relic:
                relicInfoCardCanvas.ChangeState(false,true,false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
        }
    }
}
