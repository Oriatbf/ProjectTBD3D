using System;
using System.Collections.Generic;
using System.Linq;
using SkillData;
using Unity.VisualScripting;
using UnityEngine;

public class SkillStackInfo
{
    public float stackTurn;
    public SkillBase skill;
    public Tile sourceTile;
    public Team team;

    public SkillStackInfo ()
    {

    }
    
    public SkillStackInfo (SkillStackInfo skillStackInfo)
    {
        stackTurn = skillStackInfo.stackTurn;
        skill = skillStackInfo.skill.Clone();
        sourceTile = skillStackInfo.sourceTile;
        team = skillStackInfo.team;
    }
}

public class SheetDataManager : Singleton<SheetDataManager>
{
    private List<SkillData.SkillBase> skillList = new List<SkillData.SkillBase>();
    private void Awake()
    {
        SkillData.Data.Load();
        UnitData.Data.Load();
        
        foreach (var data in SkillData.Data.DataList)
        {
            SkillData.SkillBase newSkill = new SkillData.SkillBase(data);
            skillList.Add(newSkill);
        }
        
        DIContainer.RegisterService(this);
    }
    public UnitData.Data GetUnitData(int id) => UnitData.Data.DataList.FirstOrDefault(d=>d.Id==id);
    public SkillBase GetSkillBase(int id) => skillList.FirstOrDefault(s=>s.GetData().ID==id).Clone();

    public List<SkillBase> GetSkillBaseList(List<int> ids)
        => skillList.Where(s => ids.Contains(s.GetData().ID)).Select(s=>s.Clone()).ToList();
    

}
