using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class UnitController : MonoBehaviour
{
    protected Unit _unit;
    protected Tile curTile;

    protected virtual void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    public void Initialize(Tile tile)
    {
        this.curTile = tile;
    }
    

}
