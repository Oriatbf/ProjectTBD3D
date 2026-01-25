using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utility;
using GoogleSheet.Core.Type;
using SkillData;


[UGS(typeof(Rarity))]
public enum Rarity
{
    Common,Rare,Epic,Legendary
}

[UGS(typeof(ManualType))]
public enum ManualType
{
    //Manual은 스크립트의 값 지정으로만 가져올 수 있음
    None,Manual
}

public class SheetDataManager : Singleton<SheetDataManager>
{
    //전체 SkillBase 저장
    private List<SkillData.SkillBase> skillList = new List<SkillData.SkillBase>();
    private List<RelicBase> relicList = new List<RelicBase>();
    private void Awake()
    {
        SkillData.Data.Load();
        UnitData.Data.Load();
        RelicData.Data.Load();
        
        //엑셀의 데이터를 리스트에 저장
        foreach (var data in SkillData.Data.DataList)
        {
            SkillData.SkillBase newSkill = new SkillData.SkillBase(data);
            skillList.Add(newSkill);
        }

        foreach (var data in RelicData.Data.DataList)
        {
            RelicBase newRelic = new RelicBase(data);
            relicList.Add(newRelic);
        }
        
    }
    
    //특정 ID의 유닛 엑셀데이터 가져오기
    public UnitData.Data GetUnitData(int id) => UnitData.Data.DataList.FirstOrDefault(d=>d.Id==id);
    
    //랜덤 유닛 엑셀데이터 가져오기
    public List<UnitData.Data> GetRandomUnitData(int count)
    {
        var list = UnitData.Data.DataList.Where(s=>s.ManualType == ManualType.None).ToList();
        return list.NonDupRandomT(count);
    }

    public List<RelicBase> GetRelicDataByIds(List<int> ids)
    {
        var list =  relicList.Where(s => ids.Contains(s.GetData().ID)).Select(s=>s.Clone()).ToList();
        return list;
    }
    
    public List<RelicBase> GetRandomRelicData(int count)
    {
        var list = relicList.Where(s=>s.GetData().ManualType == ManualType.None).ToList();
        return list.NonDupRandomT(count);
    }

    public List<RelicBase> GetRelicList => relicList;
    
    #region Skills

    //특정 ID의 스킬베이스 가져오기
    public SkillBase GetSkillBase(int id) => skillList.FirstOrDefault(s=>s.GetData().ID==id).Clone();

    //id리스트로 스킬베이스들 가져오기
    public List<SkillBase> GetSkillBaseList(List<int> ids)
        => skillList.Where(s => ids.Contains(s.GetData().ID)).Select(s=>s.Clone()).ToList();

    //랜덤 스킬베이스 가져오기
    public List<SkillBase> GetRandomSkillBaseList(int count)
    {
        var list = skillList.Where(s=>s.GetData().ManualType == ManualType.None).Select(s=>s.Clone()).ToList();
        return list.NonDupRandomT(count);
    }

    //전체 스킬베이스 가져오기
    public List<SkillBase> GetAllSkills() => skillList;

    #endregion
}
