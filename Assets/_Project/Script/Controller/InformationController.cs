using Cysharp.Threading.Tasks;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InformationController : BaseController
{
    private readonly string CanvasPath = "Assets/_Project/Prefab/UI/Skill/SkillInfoCanvas.prefab";
    private readonly string SkillCardPath = "Assets/_Project/Prefab/UI/Skill/SkillInfomationCard.prefab";
    private readonly string UnitCardPath = "Assets/_Project/Prefab/UI/Skill/UnitInfomationCard.prefab";
    private Panel panel;
    private SkillInformationCard skillCardInfo;
    private UnitInformationCard unitCardInfo;
    private bool isShow = false;
    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    private async void SetCanvas()
    {
        var canvas = await Addressables.LoadAssetAsync<GameObject>(CanvasPath);
        var canvasTrans = GameObject.Instantiate(canvas).transform;
        var _panel = canvasTrans.GetComponentInChildren<Panel>();
        if (_panel != null)
        {
            panel = _panel;
            var card = await Addressables.LoadAssetAsync<GameObject>(SkillCardPath);
            var cardTrans =  GameObject.Instantiate(card,_panel.transform).transform;
            if (cardTrans.TryGetComponent(out SkillInformationCard skillCardInfo))
            {
                this.skillCardInfo = skillCardInfo;
            }
            
            var unitCard = await Addressables.LoadAssetAsync<GameObject>(UnitCardPath);
            var unitCardTrans = GameObject.Instantiate(unitCard,_panel.transform).transform;
            if(unitCardTrans.TryGetComponent(out UnitInformationCard unitCardInfo))
                this.unitCardInfo = unitCardInfo;
        }

    }

    public void InitSkillData(SkillBase skillBase,Vector3 targetPos)
    {
        skillCardInfo.InitData(skillBase, targetPos);
        Show();
    }

    public void InitUnitData(UnitData.Data unitData, Vector3 targetPos)
    {
        unitCardInfo.InitData(unitData, targetPos);
        //Show();
    }
    
    public void Show()
    {
        if(isShow)return;
        isShow = true;
        if(panel == null)Debug.LogError("panel이 없음");
        panel.SetPosition(PanelStates.Show,true);
    }

    public void Hide()
    {
        isShow = false;
        panel.SetPosition(PanelStates.Hide,true);
    }
}
