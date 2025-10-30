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
    public Stat barrier,curCharm;
    public Stat charm;
    public Stat charmResist;

    public StatContainer() { }
    public StatContainer(UnitData.Data unitData)
    {
        var data = unitData;
        str = Stat.Create(data.Strength);
        hp = Stat.Create(data.Hp);
        turnGauge = Stat.Create(data.TurnGauge);
        avoidance = Stat.Create(data.Avoidance);
        intelligence = Stat.Create(data.Intelligence);
        barrier = Stat.Create(0);
        charm = Stat.Create(data.Charm);
        charmResist = Stat.Create(data.CharmResist);
    }
}
