using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Map;
using Newtonsoft.Json;
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
    public List<Room> mapDict =new List<Room>();
    public bool isMapGenerated = false;
    public Vector2 curStageIndex=Vector2.zero;
}

[Serializable]
public class SoundData
{
    public float masterVolume,bgmVolume,sfxVolume;

    public SoundData()
    {
        masterVolume = 0.5f;
        bgmVolume = 0.5f;
        sfxVolume = 0.5f;
    }
}

[Serializable]
public class GameData
{
    public List<UnitSaveData> units = new List<UnitSaveData>();
    public MapData mapData = new MapData();
    public SoundData SoundData = new SoundData();
    public int gold;
    public int constId = 0;
    public int mainCharacterID = 0;
}

public class DataManager : SingletonDontDestroyOnLoad<DataManager>
{
    string path;
    public GameData Data;
    private Action saveAction;
    private string fileName = "GameData.json";
    private bool isNewData = false;


    protected override void Awake()
    {
        base.Awake();
        path = Path.Combine(Application.persistentDataPath, fileName);
        JsonLoad();
        saveAction = JsonSave;
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TBDLogger.CommandLog(KeyCode.F5,this);
            Reset();
        }
        
        if (Input.GetKeyDown(KeyCode.F7))
        {
            TBDLogger.CommandLog(KeyCode.F7,this);
            JsonSave();
        }
    }

    #region 데이터 저장 관련
    private void JsonLoad()
    {
        if (!File.Exists(path))
        {
            isNewData = false;
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
            isNewData = true;
            string loadJson = File.ReadAllText(path);
            Data = JsonUtility.FromJson<GameData>(loadJson);
            //Data = JsonConvert.DeserializeObject<GameData>(loadJson);
        }
    }

    public void JsonSave()
    {
        string json = JsonUtility.ToJson(Data, true);
        //string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
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
            bringSkills = unit.GetOriginalBringSkills()
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
    public void SaveMapData(List<Room> mapData)
    {
        Data.mapData.mapDict = mapData;
        Data.mapData.isMapGenerated = true;
        saveAction?.Invoke();
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
    
    public void SaveStageIndex(Vector2Int index)
    {
        Data.mapData.curStageIndex = index;
    }

    public MapData GetMapData() => Data.mapData;

    #region SoundDataAPI

    public SoundData GetSoundData() => Data.SoundData;
    public void SaveMasterVolume(float value)
    {
        Data.SoundData.masterVolume = value;

    }

    public void SaveBGMVolume(float value)
    {
        Data.SoundData.bgmVolume = value;
   
    }

    public void SaveSFXVolume(float value)
    {
        Data.SoundData.sfxVolume = value;
    }

    #endregion

    public bool IsNewData() => isNewData;
    
    public int GetConstId() => Data.constId;
    public void SetConstID(int id) => Data.constId = id;

    public void SetMainCharcter(int id)
    {
        Reset();
        JsonLoad();//Load안에 파일이 없으면 새로 생성하는 기능이 있음
        Data.units.Add(new UnitSaveData()
        {
            iconKey = UnitData.Data.DataList[id].AnimatorName,
            constId = RandomID.GetConstID(),
            id = id,
            bringSkills = UnitData.Data.DataList[id].BringSkill,
            statContainer = new StatContainer(UnitData.Data.DataList[id])
        });
        JsonSave();
        
    }

    public int GetGold()=>Data.gold;

    public RoomType GetCurRoomType()
    {
        if (!Data.mapData.isMapGenerated)
        {
            Debug.LogError("현재 맵이 생성되어 있지 않음");
            return RoomType.Enemy;
        }
        var curIndex = Data.mapData.curStageIndex;
        var targetRoom = Data.mapData.mapDict.Find(s => s._index == curIndex);
        if (targetRoom == null)
        {
            Debug.LogError("현재 룸 데이터가 없음");
            return RoomType.Enemy;
        }
        return targetRoom._roomType;
    }
    
    public void SetGold(int value)
    {
        Data.gold = value;
        JsonSave();
    }
}
