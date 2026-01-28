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

        public float stackTurn = 0;
        public TargetType targetType { get; set; }

        public void Init(TargetType targetType,int rowCount, int columnCount)
        {
            this.targetType = targetType;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
        }

        public SkillContext() { }

        public SkillContext(SkillContext originalSkillContext)
        {
            SourceTile = originalSkillContext.SourceTile;
            SourceUnit = originalSkillContext.SourceUnit;
            TargetTile = originalSkillContext.TargetTile;
            TargetUnit = originalSkillContext.TargetUnit;
            rowCount = originalSkillContext.rowCount;
            columnCount = originalSkillContext.columnCount;
            targetType = originalSkillContext.targetType;
        }

        public void InitSourceTile(Tile sourceTile)
        {
            this.SourceTile = sourceTile;
            if(sourceTile.GetUnit() == null)Debug.LogError("sourceUnit is null");
            SourceUnit = sourceTile.GetUnit();
            
        }

        public void InitStackTurn(float stackTurn)
        {
            this.stackTurn = stackTurn;
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
            var tileController = ApplicationManager.Inst.GetModule<TileController>();
            switch (targetType)
            {
                case TargetType.Area:
                    _tiles = tileController.GetTiles(TargetTile,rowCount,columnCount);
                    break;
                case TargetType.Source:
                    break;
                case TargetType.All:
                    _tiles = tileController.GetAllTiles();
                    break;
            }
            
            return _tiles;
        }
        
    }
}