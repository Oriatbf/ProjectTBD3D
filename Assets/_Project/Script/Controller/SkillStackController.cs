using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillStackController : BaseController
{
    private readonly string skillIconPath = "Assets/_Project/Prefab/UI/Skill/SkillIcon.prefab";
    private readonly string SkillStackCanvasPath = "Assets/_Project/Prefab/UI/SkillStackCanvas.prefab";
    private Transform canvas;
    private SkillIcon skillIcon;
    private Camera _camera;

    public override void OnInitialize()
    {
        base.OnInitialize();
        _camera = Camera.main;
        SetPrefab();
    }

    private async void SetPrefab()
    {
        var _canvas = await Addressables.LoadAssetAsync<GameObject>(SkillStackCanvasPath).ToUniTask();
        var obj = GameObject.Instantiate(_canvas);
        this.canvas = obj.transform.GetChild(0).transform;
        
        var _skillIcon = await Addressables.LoadAssetAsync<GameObject>(skillIconPath).ToUniTask();
        if(_skillIcon.TryGetComponent(out SkillIcon skillIcon))this.skillIcon = skillIcon;
    }

    public void StackSkill(SkillData.SkillBase skill, Vector3 pos)
    {
        var obj = GameObject.Instantiate(skillIcon,canvas);
        obj.Init(skill);
        Vector3 screenPos = _camera.WorldToScreenPoint(pos);
        obj.transform.position = screenPos+new Vector3(0,400);
    }
}
