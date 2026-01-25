using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class StatContainer
{
    public Stat str, hp;
    public Stat turnGauge;
    public Stat avoidance;
    public Stat intelligence;
    public Stat barrier;
    public Stat charm;
    public Stat charmResist;

    public StatContainer() { }
    public StatContainer(UnitData.Data unitData)
    {
        var data = unitData;
        str = Stat.Create(0);
        intelligence = Stat.Create(0);
        barrier = Stat.Create(0);
        
        hp = Stat.Create(data.Hp,data.Hp);
        turnGauge = Stat.Create(0,data.TurnGauge);
        avoidance = Stat.Create(data.Avoidance);
        charm = Stat.Create(data.Charm);
        charmResist = Stat.Create(0,data.CharmResist);
    }
}
