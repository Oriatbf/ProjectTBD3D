using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillData
{
    public class SkillContext
    {
        public Tile SourceTile { get; set; }
        public Unit SourceUnit { get; set; }
        public Tile TargetTile { get; set; }
        public Unit TargetUnit { get; set; }
        
        public int rowCount=0,columnCount=0;
        public TargetType targetType { get; set; }

        public void Init(TargetType targetType,int rowCount, int columnCount)
        {
            this.targetType = targetType;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
        }

        public void InitSourceTile(Tile sourceTile)
        {
            this.SourceTile = sourceTile;
            SourceUnit = sourceTile.GetUnit();
        }

        public void InitTargetTile(Tile targetTile)
        {
            this.TargetTile = targetTile;
            TargetUnit = targetTile.GetUnit();
        }
        
        public void ForEachTarget(Action<Unit> action)
        {
            foreach (var targetTile in GetTargetTiles())
            {
                var targetUnit = targetTile.GetUnit();
                if (targetUnit != null)
                {
                    action?.Invoke(targetUnit);
                }
            }
        }

        private List<Tile> GetTargetTiles()
        {
            List<Tile> _tiles = new List<Tile>();
            switch (targetType)
            {
                case TargetType.Area:
                    _tiles = TileManager.Inst.GetTiles(TargetTile,rowCount,columnCount);
                    break;
                case TargetType.Source:
                    break;
                case TargetType.All:
                    _tiles = TileManager.Inst.GetAllTiles();
                    break;
            }
            
            return _tiles;
        }
        
    }
}