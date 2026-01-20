using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BuffInfoController : BaseController
{
    private readonly string CanvasPath = "Assets/_Project/Prefab/UI/Buff/BuffInfoCanvas.prefab";
    private readonly string BuffUIPath = "Assets/_Project/Prefab/UI/Buff/BuffInfoUI.prefab";
    private Panel panel;
    private BuffInfoUI buffInfo;
    private bool isShow = false;
    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    private async void SetCanvas()
    {
        var canvas = await Addressables
            .LoadAssetAsync<GameObject>(CanvasPath);
        var canvasTrans = GameObject.Instantiate(canvas).transform;
        var _panel = canvasTrans.GetComponentInChildren<Panel>();
        if (_panel != null)
        {
            panel = _panel;
            var buffUI = await Addressables.LoadAssetAsync<GameObject>(BuffUIPath);
            var buffUITrans =  GameObject.Instantiate(buffUI,_panel.transform).transform;
            if (buffUITrans.TryGetComponent(out BuffInfoUI buffInfo))
            {
                this.buffInfo = buffInfo;
            }
        }

    }

    public void InitData(ActionState actionState,Vector3 targetPos)
    {
        string info = $"이름은 : {actionState.GetData().id} 턴은 : {actionState.GetData().turn} 스택은 :{actionState.GetData().stack}";
        buffInfo.InitData(info, targetPos);
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
