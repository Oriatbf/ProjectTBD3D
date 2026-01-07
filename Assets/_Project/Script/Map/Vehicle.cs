using System;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField]private Vector2Int curIndex = Vector2Int.zero;
    [SerializeField] private Transform wheelParent;
    [SerializeField] private float rotationSpeed = 100f; // 회전 속도 조절
    
    private List<Transform> wheels = new List<Transform>();
    bool isMoving = false;

    private void Awake()
    {
        foreach (Transform wheel in wheelParent)
        {
            wheels.Add(wheel);
        }
    }

    private void Update()
    {
        if(isMoving)Rotating();
        ClickRoomTile();
    }

    private void ClickRoomTile()
    {
        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                // RoomTile 클릭 처리
                if (hit.collider.TryGetComponent<RoomTile>(out var roomTile))
                {
                    if (roomTile.GetRoomState() != RoomState.Reachable) return;
                    curIndex =  roomTile.GetRoom().GetIndex();
                    transform.position = new Vector3(curIndex.x*5,transform.position.y , curIndex.y*5);
                    ApplicationManager.Inst.GetModule<MapController>().ResetAllRoomTileState();
                    ApplicationManager.Inst.GetModule<MapController>().EnterRoom(roomTile);
                }
                
            }
        }
    }

    private void OnMouseDown()
    {
        ApplicationManager.Inst.GetModule<MapController>().ShowReachableMap(curIndex,2);
    }

    public void SetMoving(bool isMoving)=> this.isMoving = isMoving;

    private void Rotating()
    {
        // 모든 바퀴를 x축 방향으로 회전
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(0, rotationSpeed * Time.deltaTime, 0f);
        }
    }
}
