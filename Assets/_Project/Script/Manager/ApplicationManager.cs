using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Pooling;
using _Project.Script.Controller;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ApplicationManager : Singleton<ApplicationManager>
{
    private Dictionary<Type, BaseController> _modulesByType = new Dictionary<Type, BaseController>();

    private async void Start()
    {
        await CreateController();
        foreach (var value in _modulesByType.Values)
        {
            Debug.Log(value.GetType());
            value.OnInitialize();
        }
    }

    private void Update()
    {
        foreach (var value in _modulesByType.Values)
        {
            value.OnUpdate();
        }
    }

    private void LateUpdate()
    {
        foreach (var value in _modulesByType.Values)
        {
            value.OnLateUpdate();
        }
    }

    private void FixedUpdate()
    {
        foreach (var value in _modulesByType.Values)
        {
            value.OnFixedUpdate();
        }
    }

    private async UniTask CreateController()
    {
        await InitializeModules();
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var moduleList = _modulesByType.ToList();
        for(int i = 0;i<moduleList.Count;i++)
        {
            var pair = moduleList[i];
            if (!pair.Value.ControllerInfo.ContainSceneNames.Contains(sceneName) && !pair.Value.ControllerInfo.ContainSceneNames.Contains("All"))
            {
                UnRegisterModule(pair.Key);
            }
        }
        Debug.Log($"Create Controller {_modulesByType.Count}");
    }

    private async UniTask InitializeModules()
    {
        _modulesByType.Clear();
        _modulesByType.Add(typeof(AudioController), new AudioController());
        _modulesByType.Add(typeof(PoolController), new PoolController());
        _modulesByType.Add(typeof(CanvasController), new CanvasController());
        //--------------------------------------------------------------------
        _modulesByType.Add(typeof(TileController), new TileController());
        _modulesByType.Add(typeof(RelicController), new RelicController());
        _modulesByType.Add(typeof(TitleFlowController), new TitleFlowController());
        _modulesByType.Add(typeof(SkillTurnCounterController), new SkillTurnCounterController());
        _modulesByType.Add(typeof(TurnController), new TurnController());
        _modulesByType.Add(typeof(EnemyRegisterController),new EnemyRegisterController());
        _modulesByType.Add(typeof(CharacterSkillController), new CharacterSkillController());
        _modulesByType.Add(typeof(PopUpUIController), new PopUpUIController());
        _modulesByType.Add(typeof(SkillStackController),new SkillStackController());
        _modulesByType.Add(typeof(PlayerSpawnController),new PlayerSpawnController());
        _modulesByType.Add(typeof(LootController), new LootController());
        _modulesByType.Add(typeof(InformationController), new InformationController());
        _modulesByType.Add(typeof(SettingController),new SettingController());
        _modulesByType.Add(typeof(ShopController), new ShopController());
        _modulesByType.Add(typeof(ActionStateStackController), new ActionStateStackController());
        _modulesByType.Add(typeof(BuffInfoController), new BuffInfoController());
        _modulesByType.Add(typeof(TopInfoController), new TopInfoController());
        _modulesByType.Add(typeof(SkillChangeController), new SkillChangeController());
        _modulesByType.Add(typeof(DebugController), new DebugController());
        _modulesByType.Add(typeof(CameraController), new CameraController());
        _modulesByType.Add(typeof(MapCameraMoveController), new MapCameraMoveController());
       // _modulesByType.Add(typeof(MapController), new MapController());
        _modulesByType.Add(typeof(GameFlowController), new GameFlowController());
    }

    private void UnRegisterModule(Type type)
    {
        _modulesByType.Remove(type);
    }

    public T GetModule<T>() where T : BaseController
    {
        if (_modulesByType.TryGetValue(typeof(T), out var module))
            return module as T;
        return null;
    }
}
