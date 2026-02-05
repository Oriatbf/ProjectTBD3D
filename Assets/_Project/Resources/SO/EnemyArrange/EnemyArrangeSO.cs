using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyArrangeType
{
    enemy,strongEnemy,Boss,Tutorial
}

[CreateAssetMenu(fileName = "EnemyArrangeSO",menuName = "Scriptable Enemy/EnemyArrange",order = 1)]
public class EnemyArrangeSO : ScriptableObject
{
    [Tooltip("소환될 Act")]public int appearAct;
    [Tooltip("난이도")] public int difficulty;
    public EnemyArrangeType enemyArrangeType;
    public List<LootData> lootDatas;

    [Serializable]
    public struct EnemyArrange
    {
        public int unitIndex;
        public Vector2 posIndex;
    }
    public List<EnemyArrange> EnemyArranges = new List<EnemyArrange>();
    
}


[Serializable]
public class LootData
{
    public enum LootType
    {
        Skill,Gold,Item
    }
    
    public LootType lootType;

    public int value;
}
