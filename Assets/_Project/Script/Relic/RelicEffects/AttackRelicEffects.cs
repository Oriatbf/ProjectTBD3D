using UnityEngine;

namespace _Project.Script.Relic
{
    public class Thunder : RelicEffect
    {
        public override void Excute()
        {
            var targetTile = ApplicationManager.Inst.GetModule<TileController>().GetRandomTile();
            if (targetTile.GetUnit() != null)
            {
                targetTile.GetUnit().GetDamage(999,null,SkillType.Attack);
            }
            Debug.Log("천둥번개");
        }
    }
}