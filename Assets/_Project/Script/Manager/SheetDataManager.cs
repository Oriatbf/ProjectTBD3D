using System;
using System.Collections.Generic;
using System.Linq;
using SkillData;
using UnityEngine;

public class SheetDataManager : MonoBehaviour
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
    public SkillBase GetSkillBase(int id) => skillList.FirstOrDefault(s=>s.GetData().ID==id);

}
