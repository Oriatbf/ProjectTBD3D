using UnityEngine;

public class Tile : MonoBehaviour
{
    private Unit unit;
    private Transform dangerLine;

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }
    
    public Vector3 GetPos() => transform.position;
    
}
