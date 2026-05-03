using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Project.Script.Controller;
using Core.Utility;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// json에 저장하기 위한 unit 데이터
/// </summary>
[Serializable]
public class UnitSaveData
{
    public int constId;
    public int id;
    public List<int> bringSkills = new List<int>();
    public StatContainer statContainer;
    public string animatorName;
    public Rarity rarity;
    public float charm;
    public float flipDir = 0;
    public ConsumptionType consumptionType;

    public UnitSaveData() { }
    public UnitSaveData(UnitData.Data unitData)
    {
        animatorName = unitData.AnimatorName;
        constId = RandomID.GetConstID();
        id = unitData.Id;
        bringSkills = unitData.BringSkill;
        statContainer = new StatContainer(unitData);
        rarity = unitData.Rarity;
        charm = unitData.Charm;
        flipDir = unitData.Flip;
        consumptionType = unitData.ConsumptionType;
    }

}

[Serializable]
public class MapData
{
    public List<List<MapNode>> mapDict =new List<List<MapNode>>();
    public bool isMapGenerated = false;
    public NodeCoord curNodeCoord = new NodeCoord(-1,0,NodeType.None);
    public NodeCoord prevNodeCoord = new NodeCoord(); //클리어 시 예상노드
    public int curFloor = 0;
}

[Serializable]
public class RelicSaveData
{
    public List<int> relicIDList = new List<int>();

    public void AddRelic(int id)
    {
        relicIDList.Add(id);
    }

    public void DeleteRelic(int id)
    {
        for (int i = relicIDList.Count-1; i >=0; i--)
        {
            if (relicIDList[i] == id)
            {
                relicIDList.RemoveAt(i);
                return;
            }
        }
        
        Debug.Log("해당 아이디를 가진 유물이 없음");
    }
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
    public SoundData soundData = new SoundData();
    public RelicSaveData relicSaveData = new RelicSaveData();
    
    public int gold;
    public int constId = 0;
    public int mainCharacterID = 0;
    public bool isNewData = false;
    public bool isFirstGame = true;
    public bool autoTarget = false;

    public void Reset()
    {
        units.Clear();
        mapData = new MapData();
        relicSaveData = new RelicSaveData();
        gold = 0;
        constId = 0;
        mainCharacterID = 0;
        isNewData = false;
        isFirstGame = true;
    }
}

public class DataManager : SingletonDontDestroyOnLoad<DataManager>
{
    string path;
    public GameData Data;
    private Action saveAction;
    private string fileName = "GameData.json";
    public bool debugMode = false;
    public bool isTutorial = false;


    protected override void Awake()
    {
        base.Awake();
        SkillData.Data.Load();
        UnitData.Data.Load();
        RelicData.Data.Load();
        path = Path.Combine(Application.persistentDataPath, fileName);
        JsonLoad();
        saveAction = JsonSave;   
    }

    


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TBDLogger.CommandLog(KeyCode.M,this);
            Data.autoTarget = !Data.autoTarget;
        }
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (!TBDLogger.CommandLog(KeyCode.F5,this))return;
            FileReset();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!TBDLogger.CommandLog(KeyCode.S, this)) return;
            Data.mapData.curNodeCoord.type = NodeType.Shop;
            FadeInFadeOutManager.Inst.FadeOut("GamePlay");
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!TBDLogger.CommandLog(KeyCode.E, this)) return;
            Data.mapData.curNodeCoord.type = NodeType.Event;
            FadeInFadeOutManager.Inst.FadeOut("GamePlay");
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!TBDLogger.CommandLog(KeyCode.W, this)) return;
            Data.mapData.curNodeCoord.type = NodeType.Enemy;
            FadeInFadeOutManager.Inst.FadeOut("GamePlay");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!TBDLogger.CommandLog(KeyCode.T, this)) return;
            Data.mapData.curNodeCoord.type = NodeType.TestEnemy;
            FadeInFadeOutManager.Inst.FadeOut("GamePlay");
        }
        
        
        if (Input.GetKeyDown(KeyCode.F7))
        {
            if (!TBDLogger.CommandLog(KeyCode.F7,this))return;
            JsonSave();
        }

        var k = Input.GetKey(KeyCode.O);
        var j = Input.GetKey(KeyCode.R);
        if (Input.GetKeyDown(KeyCode.I) && k && j)
        {
            debugMode = !debugMode;
            Debug.Log($"디버그모드 {debugMode}");
        }


        if (debugMode && Input.GetKeyDown(KeyCode.Keypad5))
        {
            if (!TBDLogger.CommandLog(KeyCode.Keypad5,this))return;
            TimeScaler.ChangeTimeScale(5);
        }
        
        if (debugMode && Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (!TBDLogger.CommandLog(KeyCode.Keypad1,this))return;
            TimeScaler.ChangeTimeScale(1);
        }
    }

    #region 데이터 저장 관련
    private void JsonLoad()
    {
        if (!File.Exists(path))
        {
            Data = new GameData();
            Data.isFirstGame = true;
            Data.isNewData = true; 
            JsonSave();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            Data = JsonConvert.DeserializeObject<GameData>(loadJson);
            /*
            string loadJson = File.ReadAllText(path);
            Data = JsonUtility.FromJson<GameData>(loadJson);
           */
        }
    }

    public void JsonSave()
    {
       // string json = JsonUtility.ToJson(Data, true);
        string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
        File.WriteAllText(path, json);
    }
    
    public void  FileReset()
    {
        if (File.Exists(path))
        {
            File.Delete(path); // 파일 삭제
        }
        
    }
    
    public void DataReset()
    {
        var autoTarget = Data.autoTarget;
        //var nodeCoord = Data.mapData.curNodeCoord;
        Data.Reset();
        Data.isFirstGame = false;
        Data.isNewData = true;
        Data.autoTarget = autoTarget;
        //Data.mapData.curNodeCoord = nodeCoord;
        JsonSave();
    }
    #endregion
    
    
    #region 캐릭터저장관련

    /// <summary>
    /// 유닛 스킬 덮어쓰기
    /// </summary>
    public void SaveUnitSkills(int constID,List<int> skillList)
    {
        if (ApplicationManager.Inst.GetModule<GameFlowController>().GetCurNodeType() == NodeType.Tutorial) return;
        var unit = Data.units.FirstOrDefault(u=>u.constId==constID);
        unit.bringSkills = skillList;
        JsonSave();
    }
    public void SetMainCharcter(int id)
    {
        DataReset();
        Data.units.Add(new UnitSaveData(UnitData.Data.DataList[id]));
        Data.mainCharacterID = id;
        Data.isNewData = false;
        JsonSave();
        Debug.Log("MainChaarcter초뭏ㄷ");
    }

    public void SetCharacter(int id)
    {
        Data.units = new List<UnitSaveData>();
        Data.units.Add(new UnitSaveData(UnitData.Data.DataList[id]));
        Data.mainCharacterID = id;
        Data.isNewData = false;
        JsonSave();
    }
    
    public List<UnitSaveData> GetAllSavedUnits() => Data.units;
    public UnitSaveData GetSavedUnit(int constID)=>Data.units.FirstOrDefault(u => u.constId == constID);

    /// <summary>
    /// 살아남은 플레이어 유닛 저장, 매혹당한 적 저장
    /// </summary>
    /// <param name="unit"></param>
    public void SaveSurviveUnit(Unit unit)
    {
        if (ApplicationManager.Inst.GetModule<GameFlowController>().GetCurNodeType() == NodeType.Tutorial) return;
        if (Data.units.Count >= 8) return;
        var unitSaveData = unit.GetUnitData();
        var originalData = SheetDataManager.Inst.GetUnitData(unitSaveData.id);
        var originalStatContainer = new StatContainer(originalData);
        UnitSaveData newUnitSaveData = new UnitSaveData()
        {
            constId = unitSaveData.constId,
            id= unitSaveData.id,
            animatorName = unitSaveData.animatorName,
            statContainer = originalStatContainer,
            bringSkills = unit.GetOriginalBringSkills(),
            consumptionType = ConsumptionType.NonConsumable,
            flipDir = unitSaveData.flipDir,
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

    /// <summary>
    /// Shop, event전용
    /// </summary>
    /// <param name="unitSaveData"></param>
    public void SaveUnit(UnitSaveData unitSaveData)
    {
        if (Data.units.Count >= 8) return;
        if(unitSaveData == null)Debug.LogError("unitSaveData = null");
        Data.units.Add(unitSaveData);
    }

    public void SaveUnit(int id)
    {
        if (Data.units.Count >= 8) return;
        var originalData = SheetDataManager.Inst.GetUnitData(id);
        var originalStatContainer = new StatContainer(originalData);
        UnitSaveData newUnitSaveData = new UnitSaveData()
        {
            constId = RandomID.GetConstID(),
            id= id,
            animatorName = originalData.AnimatorName,
            statContainer = originalStatContainer,
            bringSkills = originalData.BringSkill,
            flipDir = originalData.Flip,
            consumptionType = ConsumptionType.NonConsumable,
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
            Data.units.Remove(unit);
        }
    }
    #endregion
    
    
    #region MapAPI
    
    /// <summary>
    /// 맵 상태 저장
    /// </summary>
    public void SaveMapData(List<List<MapNode>> mapData)
    {
        Data.mapData.mapDict = mapData;
        Data.mapData.isMapGenerated = true;
        saveAction?.Invoke();
    }


    public void SaveCurNodeType(NodeCoord prevNodeCoord)
    {
        Data.mapData.prevNodeCoord = prevNodeCoord;
        saveAction?.Invoke();
    }

    public void ClearMap()
    {
        Data.mapData.curFloor += 1;
        Data.mapData.curNodeCoord = Data.mapData.prevNodeCoord;
        saveAction?.Invoke();
    }

    public MapData GetMapData() => Data.mapData;

    #endregion

    #region SoundDataAPI

    public SoundData GetSoundData() => Data.soundData;
    public void SaveMasterVolume(float value)
    {
        Data.soundData.masterVolume = value;

    }

    public void SaveBGMVolume(float value)
    {
        Data.soundData.bgmVolume = value;
   
    }

    public void SaveSFXVolume(float value)
    {
        Data.soundData.sfxVolume = value;
    }

    #endregion

    public bool IsNewData() => Data.isNewData;
    public bool IsFirstGame() => Data.isFirstGame;
    
    public int GetConstId() => Data.constId;
    public void SetConstID(int id) => Data.constId = id;

    public int GetGold()=>Data.gold;
    
    public RelicSaveData GetRelicSaveData() => Data.relicSaveData;

    public void SaveRelic(int id)
    {
        Data.relicSaveData.AddRelic(id);
    }

    public void SetGold(int value)
    {
        if (ApplicationManager.Inst.GetModule<GameFlowController>().GetCurNodeType() == NodeType.Tutorial) return;
        Data.gold = value;
        JsonSave();
    }

    public void HealAll(int value)
    {
        for (int i = 0; i < Data.units.Count; i++)
        {
            Data.units[i].statContainer.hp.AddBaseValue(value);
        }
    }
}
