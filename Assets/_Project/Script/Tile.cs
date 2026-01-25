using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Unit unit;
    [SerializeField] private Transform dangerLine;
    private Vector2 index;

    private void Start()
    {
        dangerLine.gameObject.SetActive(false);
    }

    public void SetUnit(Unit unit) => this.unit = unit;
    public void SetIndex(Vector2 index) => this.index = index;
    public Unit GetUnit() => unit;
    public Vector3 GetPos() => transform.position;
    public Vector2 GetIndex()=> index;
    
    public void DestroyUnit()=> unit = null;

    public void Target()
    {
        dangerLine.gameObject.SetActive(true);
    }

    public void UnTarget()
    {
        dangerLine.gameObject.SetActive(false);
    }
    
}
