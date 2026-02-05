using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : BaseController
{
    private Camera _camera;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private float fov = 0; //Camera Field Of View
    
    private readonly float clampRotY = 2.5f;
    private readonly float dur = 0.75f;
    
    public override ControllerInfo ControllerInfo { get; } = new()
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
        _camera = Camera.main;
        fov = _camera.fieldOfView;
        originalPos = _camera.transform.position;
        originalRot = _camera.transform.rotation;
    }

    public async  UniTask TargetLook(Unit unit)
    {
        _camera.DOKill();
       
        Vector3 direction = unit.transform.position - originalPos;
        direction.y = 0;
        
        float targetYAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        
        float originalYAngle = Mathf.DeltaAngle(0, originalRot.eulerAngles.y);
        
        float deltaY = Mathf.DeltaAngle(originalYAngle, targetYAngle);
        // deltaY의 절댓값을 0~1로 정규화 (예: 최대 8도 기준)
        float normalizedDelta = Mathf.Clamp01(Mathf.Abs(deltaY) / 8f);
        float cRot = Mathf.Lerp(0, clampRotY, normalizedDelta);
    
        deltaY = Mathf.Clamp(deltaY, -cRot, cRot);

        Vector3 euler = originalRot.eulerAngles;
        euler.y = originalYAngle + deltaY;
        _camera.DOFieldOfView(fov - 1, dur).SetEase(Ease.OutQuad);
         await _camera.transform.DORotate(euler, dur).SetEase(Ease.OutQuad).ToUniTask();
         await UniTask.WaitForSeconds(.1f);
        _camera.DOFieldOfView(fov, dur).SetEase(Ease.OutQuad).ToUniTask();
    }

    public async UniTask OriginLook()
    {
        _camera.DOKill();
        var rotTask =  _camera.transform.DORotateQuaternion(originalRot, dur).ToUniTask();
        var fovTask =  _camera.DOFieldOfView(fov, dur).SetEase(Ease.OutQuad).ToUniTask();
        await UniTask.WhenAll(rotTask, fovTask);
    }

    private void Rotate()
    {
        
    }
}
