using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : BaseController
{
    CanvasLoader _canvasLoader;
    public override void OnInitialize()
    {
        base.OnInitialize();
        _canvasLoader = Resources.Load<CanvasLoader>("Loader/CanvasLoader");
        LoadSceneCanvas(SceneManager.GetActiveScene().name);
    }

    public void LoadSceneCanvas(string sceneName)
    {
        if (_canvasLoader == null) return;
        var sceneCanvases = _canvasLoader.GetCanvasListForScene(sceneName);
        foreach (var canvasInfo in sceneCanvases)
        {
            var canvas = UnityEngine.Object.Instantiate(canvasInfo.Prefab);
            var baseCanvas = canvas.GetComponent<BaseCanvas>();
        }
    }
}
