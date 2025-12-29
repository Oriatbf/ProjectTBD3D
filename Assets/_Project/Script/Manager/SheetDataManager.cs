using System;
using System.Collections.Generic;
using System.Linq;
using SkillData;

public class SheetDataManager : Singleton<SheetDataManager>
{
    //전체 SkillBase 저장
    private List<SkillData.SkillBase> skillList = new List<SkillData.SkillBase>();
    private void Awake()
    {
        SkillData.Data.Load();
        UnitData.Data.Load();
        
        //엑셀의 데이터를 리스트에 저장
        foreach (var data in SkillData.Data.DataList)
        {
            SkillData.SkillBase newSkill = new SkillData.SkillBase(data);
            skillList.Add(newSkill);
        }
        
    }
    
    //특정 ID의 유닛 엑셀데이터 가져오기
    public UnitData.Data GetUnitData(int id) => UnitData.Data.DataList.FirstOrDefault(d=>d.Id==id);
    
    //랜덤 유닛 엑셀데이터 가져오기
    public List<UnitData.Data> GetRandomUnitData(int count) => UnitData.Data.DataList.NonDupRandomT(count);

    #region Skills

    //특정 ID의 스킬베이스 가져오기
    public SkillBase GetSkillBase(int id) => skillList.FirstOrDefault(s=>s.GetData().ID==id).Clone();

    //id리스트로 스킬베이스들 가져오기
    public List<SkillBase> GetSkillBaseList(List<int> ids)
        => skillList.Where(s => ids.Contains(s.GetData().ID)).Select(s=>s.Clone()).ToList();

    //랜덤 스킬베이스 가져오기
    public List<SkillBase> GetRandomSkillBaseList(int count) => skillList.NonDupRandomT(count);
    
    //전체 스킬베이스 가져오기
    public List<SkillBase> GetAllSkills() => skillList;

    #endregion
}
