using UnityEngine;

public class CameraFollowSmoothDeadZone : MonoBehaviour
{
    public Transform target;

    [Range(0f, 1f)]
    public float deadZoneLeft = 0.3f;   // 화면 왼쪽 30%
    [Range(0f, 1f)]
    public float deadZoneRight = 0.7f;  // 화면 오른쪽 70%

    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    private bool isMoving = false;
    private Vector3 targetCamPos;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        Move();
        if (isMoving) return;
        Vector3 viewport = cam.WorldToViewportPoint(target.position);

        // 타겟까지의 Z 거리
        float distance = Mathf.Abs(target.position.z - cam.transform.position.z);

        // 화면 절반 폭 (월드 기준)
        float halfWidth = distance * Mathf.Tan(Mathf.Deg2Rad * cam.fieldOfView * 0.5f) * cam.aspect;

         targetCamPos = cam.transform.position;

        // --- Dead-Zone 판정 -----------------------------
        bool leftOut = viewport.x < deadZoneLeft;
        bool rightOut = viewport.x > deadZoneRight;

        if (leftOut)
        {
            //targetCamPos += new Vector3(-halfWidth, 0, 0);
        }
        else if (rightOut)
        {
            targetCamPos += new Vector3(halfWidth, 0, 0);
            isMoving = true;
        }
       
        
    }
    
    private void Move()
    {
        if (!isMoving) return;
        
        cam.transform.position = Vector3.SmoothDamp(
            cam.transform.position,
            targetCamPos,
            ref velocity,
            smoothTime
        );
        if (Vector3.Distance(cam.transform.position, targetCamPos) < 0.01f)
        {
            cam.transform.position = targetCamPos;
            isMoving = false;    
        }
        
      
    }
}