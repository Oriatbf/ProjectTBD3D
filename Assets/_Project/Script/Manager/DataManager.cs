using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class UnitSaveData
{
    public int id;
    public List<int> bringSkills = new List<int>();
    public StatContainer statContainer;
}

[Serializable]
public class GameData
{
    public List<UnitSaveData> units = new List<UnitSaveData>();
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
        if(Input.GetKeyDown(KeyCode.F4))Reset();
    }


    private void JsonLoad()
    {
        if (!File.Exists(path))
        {
            Data = new GameData();
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

    public void SaveUnit(Unit unit)
    {
        var unitData = unit.GetUnitData();
        UnitSaveData unitSaveData = new UnitSaveData()
        {
            id= unitData.id,
            statContainer = unit.GetStatContainer(),
            bringSkills = unit.GetSkillList()
        };
        Data.units.Add(unitSaveData);
    }
}
