using UnityEngine;

public class MapCameraMoveController : BaseController
{
        
    private readonly float dragSpeed = 3f;
    private readonly float zoomSpeed = 5f;
    private readonly float minZoom = 10f,maxZoom = 30f;
    private Vector3 lastMousePos;
    private Camera _camera;

    public override ControllerInfo ControllerInfo { get; } = new()
    {
        ContainSceneNames = new string[] {"MapScene" },
        Priority = 0,
        UpdateInterval = 0,
        LateUpdateInterval = 1,
        FixedUpdateInterval = 0,
    };

    public override void OnInitialize()
    {
        base.OnInitialize();
        _camera = Camera.main;
    }

    public override void OnLateUpdate()
    {
        base.OnLateUpdate();
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            Vector3 move = new Vector3(delta.y, 0, -delta.x) * (dragSpeed * Time.deltaTime);
            _camera.transform.Translate(move, Space.World);
            lastMousePos = Input.mousePosition;
        }
        Zoom();
    }

    private void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Approximately(scroll, 0f))
            return;

        float newSize = _camera.orthographicSize - scroll * zoomSpeed;
        _camera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }

}
