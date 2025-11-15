using System;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
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
