using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillInformationController : BaseController
{
    private readonly string CanvasPath = "Assets/_Project/Prefab/UI/Skill/SkillInfoCanvas.prefab";
    private readonly string CardPath = "Assets/_Project/Prefab/UI/Skill/SkillInfomationCard.prefab";
    private Panel panel;
    private SkillInformationCard cardInfo;
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
            var card = await Addressables.LoadAssetAsync<GameObject>(CardPath);
            var cardTrans =  GameObject.Instantiate(card,_panel.transform).transform;
            if (cardTrans.TryGetComponent(out SkillInformationCard cardInfo))
            {
                this.cardInfo = cardInfo;
            }
        }

    }

    public void InitData(SkillStackInfo skillStackInfo,Vector3 targetPos)
    {
        cardInfo.InitData(skillStackInfo, targetPos);
        Show();
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
