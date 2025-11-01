using System;
using UnityEngine;

namespace SkillData
{
    public class SkillContext
    {
        public Tile SourceTile { get; set; }
        public Tile TargetTile { get; set; }
        public Action GiveDamage { get; set; }
        public int rowCount=0,columnCount=0;

        public void DamageToTarget(float damage)
        {
            var tiles = TileManager.Inst.GetTiles(TargetTile,rowCount,columnCount);
            foreach (var tile in tiles)
            {
                tile.GetUnit()?.GetDamage(damage);
            }
            
        }
        public void DamageToSource(float damage)
        {
            SourceTile.GetUnit().GetDamage(damage);
        }
    }
}