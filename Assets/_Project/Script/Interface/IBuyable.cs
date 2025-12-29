using UnityEngine;

public interface IBuyable
{
    public float value { get; set; }
    public abstract void Buy();
}
