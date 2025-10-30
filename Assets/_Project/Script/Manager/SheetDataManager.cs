using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SheetDataManager : MonoBehaviour
{
    private void Awake()
    {
        SkillData.Data.Load();
        UnitData.Data.Load();
        
        foreach (var data in SkillData.Data.DataList)
        {
            /*
            SkillBase newSkill = new SkillBase(data);
            if(data.SkillType == SkillType.Skill) skillList.Add(newSkill);
            else itemList.Add(newSkill);
            */
        }
        
        DIContainer.RegisterService(this);
    }
    
    public UnitData.Data GetUnitData(int id) => UnitData.Data.DataList.FirstOrDefault(d=>d.Id==id);
    public int GetSkillBase(int id) => 1;

}
