using Cysharp.Threading.Tasks;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum DataType
{
    Skill,Unit
}
public class InformationController : BaseController
{
    private SkillInfoCardCanvas _skillInfoCardCanvas;
    private UnitInfoCardCanvas _unitInfoCardCanvas;
    private bool isUnitShow = false;
    private bool isSkillShow = false;

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

    private  void SetCanvas()
    {
        var skillCardCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas("SkillInfoCardCanvas");
        if(skillCardCanvas.TryGetComponent(out SkillInfoCardCanvas skillInfoCardCanvas))
            _skillInfoCardCanvas = skillInfoCardCanvas;
        var unitInfoCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas("UnitInfoCardCanvas");
        if (unitInfoCanvas.TryGetComponent(out UnitInfoCardCanvas unitInfoCardCanvas))
            _unitInfoCardCanvas= unitInfoCardCanvas;
    }

    public void InitSkillData(SkillBase skillBase,Vector3 targetPos)
    {
        _skillInfoCardCanvas.InitData(skillBase, targetPos);
        Show(DataType.Skill);
    }

    public void InitUnitData(UnitSaveData unitSaveData, Vector3 targetPos)
    {
        var unitData = SheetDataManager.Inst.GetUnitData(unitSaveData.id);
        _unitInfoCardCanvas.InitData(unitData,unitSaveData, targetPos);
        //Show();
    }
    
    public void Show(DataType dataType)
    {
        if (dataType == DataType.Skill)
        {
            if(isSkillShow)return;
            isSkillShow = true;
            _skillInfoCardCanvas.ChangeState(true,true,true);
        }
        else if (dataType == DataType.Unit)
        {
            if(isUnitShow)return;
            isUnitShow = true;
            _unitInfoCardCanvas.ChangeState(true,true,true);
        }
    }

    public void Hide(DataType dataType)
    {
        if (dataType == DataType.Skill)
        {
            if(!isSkillShow)return;
            isSkillShow = false;
            _skillInfoCardCanvas.ChangeState(false,true);
        }
        else if (dataType == DataType.Unit)
        {
            if(!isUnitShow)return;
            isUnitShow = false;
            _unitInfoCardCanvas.ChangeState(false,true);
        }
    }
}
