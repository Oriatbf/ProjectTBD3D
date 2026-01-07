using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : BaseController
{
    CanvasLoader _canvasLoader;
    Dictionary<string, GameObject> _canvasDictionary = new Dictionary<string, GameObject>();
   
    public override ControllerInfo ControllerInfo { get; } = new()
    {
        ContainSceneNames = new string[] {"MapScene","GamePlay" },
        Priority = 0,
        UpdateInterval = 1,
        LateUpdateInterval = 1,
        FixedUpdateInterval = 1,
    };

    public override void OnInitialize()
    {
        base.OnInitialize();
        _canvasLoader = Resources.Load<CanvasLoader>("Loader/CanvasLoader");
        LoadSceneCanvas(SceneManager.GetActiveScene().name);
    }

   
    public GameObject GetCanvas(string canvasName)
    {
        if(!_canvasDictionary.ContainsKey(canvasName))Debug.LogError($"{canvasName}의 이름을 가진 canvas프래팹이 없음");
        return _canvasDictionary[canvasName];
    }

    public void LoadSceneCanvas(string sceneName)
    {
        if (_canvasLoader == null) return;
        var sceneCanvases = _canvasLoader.GetCanvasListForScene(sceneName);
        foreach (var canvasInfo in sceneCanvases)
        {
            var canvas = UnityEngine.Object.Instantiate(canvasInfo.Prefab);
            _canvasDictionary.Add(canvasInfo.Key, canvas);
        }
    }
}
