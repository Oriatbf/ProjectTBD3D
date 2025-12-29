using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Map;
using UnityEngine;

[Serializable]
public class UnitSaveData
{
    public int constId;
    public int id;
    public List<int> bringSkills = new List<int>();
    public StatContainer statContainer;
    public string iconKey;

    public UnitSaveData() { }
    public UnitSaveData(UnitData.Data unitData)
    {
        iconKey = unitData.AnimatorName;
        constId = RandomID.GetConstID();
        id = unitData.Id;
        bringSkills = unitData.BringSkill;
        statContainer = new StatContainer(unitData);
    }
}

[Serializable]
public class MapData
{
    public List<MapState> stageStates = new List<MapState>();
    public bool isMapGenerated = false;
    public int stageIndex=0;
}

[Serializable]
public class GameData
{
    public List<UnitSaveData> units = new List<UnitSaveData>();
    public MapData mapData = new MapData();
    public int gold;
    public int constId = 0;
}

public class DataManager : SingletonDontDestroyOnLoad<DataManager>
{
    string path;
    public GameData Data;
    private string fileName = "GameData.json";


    protected override void Awake()
    {
        base.Awake();
        path = Path.Combine(Application.persistentDataPath, fileName);
        JsonLoad();
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TBDLogger.CommandLog(KeyCode.F5,this);
            Reset();
        }
    }

    #region 데이터 저장 관련
    private void JsonLoad()
    {
        if (!File.Exists(path))
        {
            Data = new GameData();
            for (int i = 0; i < 2; i++)
            {
                Data.units.Add(new UnitSaveData()
                {
                    iconKey = UnitData.Data.DataList[1].AnimatorName,
                    constId = RandomID.GetConstID(),
                    id = 1,
                    bringSkills = UnitData.Data.DataList[1].BringSkill,
                    statContainer = new StatContainer(UnitData.Data.DataList[1])
                });
            }
        
            
            JsonSave();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            Data = JsonUtility.FromJson<GameData>(loadJson);
        }
    }

    public void JsonSave()
    {
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(path, json);
    }
    
    public void Reset()
    {
        if (File.Exists(path))
        {
            File.Delete(path); // 파일 삭제
        }

        // 새 데이터로 초기화
        Data = new GameData();
    }
    #endregion
    public List<UnitSaveData> GetAllSavedUnits() => Data.units;
    public UnitSaveData GetSavedUnit(int constID)=>Data.units.FirstOrDefault(u => u.constId == constID);

    /// <summary>
    /// 살아남은 플레이어 유닛 저장
    /// </summary>
    /// <param name="unit"></param>
    public void SaveUnit(Unit unit)
    {
        var unitSaveData = unit.GetUnitData();
        var originalData = SheetDataManager.Inst.GetUnitData(unitSaveData.id);
        var originalStatContainer = new StatContainer(originalData);
        UnitSaveData newUnitSaveData = new UnitSaveData()
        {
            constId = unitSaveData.constId,
            id= unitSaveData.id,
            iconKey = unitSaveData.iconKey,
            statContainer = originalStatContainer,
            bringSkills = unit.GetSkillList()
        };
        newUnitSaveData.statContainer.hp.SetBaseValue(unitSaveData.statContainer.hp._baseValue);
        
        foreach(var savedUnits in Data.units)
        {
            if (savedUnits.constId == unitSaveData.constId)
            {
                savedUnits.bringSkills = newUnitSaveData.bringSkills;
                savedUnits.statContainer = newUnitSaveData.statContainer;
                return;
            }
        }
        
        Data.units.Add(newUnitSaveData);
    }

    public void SaveUnit(UnitSaveData unitSaveData)
    {
        Data.units.Add(unitSaveData);
    }

    public void SaveUnit(int id)
    {
        var originalData = SheetDataManager.Inst.GetUnitData(id);
        var originalStatContainer = new StatContainer(originalData);
        UnitSaveData newUnitSaveData = new UnitSaveData()
        {
            constId = RandomID.GetConstID(),
            id= id,
            iconKey = originalData.AnimatorName,
            statContainer = originalStatContainer,
            bringSkills = originalData.BringSkill,
        };
        Data.units.Add(newUnitSaveData);
    }

    /// <summary>
    /// 저장된 유닛 삭제
    /// </summary>
    public void DeleteUnit(int constID)
    {
        var unit = Data.units.FirstOrDefault(u => u.constId == constID);
        if (unit != null)
        {
           // Data.units.Remove(unit);
        }
    }
    /// <summary>
    /// 맵 상태 저장
    /// </summary>
    public void SaveStageStates(List<MapState> states)
    {
        Data.mapData.stageStates = states;
        Data.mapData.isMapGenerated = true;
        JsonSave();
    }

    /// <summary>
    /// 유닛 스킬 덮어쓰기
    /// </summary>
    public void SaveUnitSkills(int constID,List<int> skillList)
    {
        var unit = Data.units.FirstOrDefault(u=>u.constId==constID);
        unit.bringSkills = skillList;
        JsonSave();
    }
    
    public void SaveStageIndex(int index)=>Data.mapData.stageIndex = index;
    public MapData GetMapData() => Data.mapData;
    
    
    
    public int GetConstId() => Data.constId;
    public void SetConstID(int id) => Data.constId = id;
    
    public int GetGold()=>Data.gold;
    
    public void SetGold(int value)
    {
        Data.gold = value;
        JsonSave();
    }
}
