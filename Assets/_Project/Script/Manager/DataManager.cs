using System;
using System.Collections.Generic;
using System.IO;
using Map;
using UnityEngine;

[Serializable]
public class UnitSaveData
{
    public int constId;
    public int id;
    public List<int> bringSkills = new List<int>();
    public StatContainer statContainer;

    public UnitSaveData() { }
    public UnitSaveData(UnitData.Data unitData)
    {
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
                    constId = RandomID.GetConstID(),
                    id = 0,
                    bringSkills = UnitData.Data.DataList[0].BringSkill,
                    statContainer = new StatContainer(UnitData.Data.DataList[0])
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
    public List<UnitSaveData> GetUnits() => Data.units;

    /// <summary>
    /// 살아남은 플레이어 유닛 저장
    /// </summary>
    /// <param name="unit"></param>
    public void SaveUnit(Unit unit)
    {
        var unitData = unit.GetUnitData();
        var originalData = SheetDataManager.Inst.GetUnitData(unitData.id);
        var originalStatContainer = new StatContainer(originalData);
        UnitSaveData newUnitSaveData = new UnitSaveData()
        {
            constId = unitData.constId,
            id= unitData.id,
            statContainer = originalStatContainer,
            bringSkills = unit.GetSkillList()
        };
        newUnitSaveData.statContainer.hp.SetBaseValue(unitData.statContainer.hp._baseValue);
        
        foreach(var savedUnits in Data.units)
        {
            if (savedUnits.constId == unitData.constId)
            {
                savedUnits.bringSkills = newUnitSaveData.bringSkills;
                savedUnits.statContainer = newUnitSaveData.statContainer;
                return;
            }
        }
        
        Data.units.Add(newUnitSaveData);
    }

    public void SaveStageStates(List<MapState> states)
    {
        Data.mapData.stageStates = states;
        Data.mapData.isMapGenerated = true;
        JsonSave();
    }
    public void SaveStageIndex(int index)=>Data.mapData.stageIndex = index;
    public MapData GetMapData() => Data.mapData;
    
    public void SetGold(int value)=>Data.gold = value;
}
