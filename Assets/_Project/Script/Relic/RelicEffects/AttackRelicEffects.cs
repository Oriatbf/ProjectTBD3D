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
                targetTile.GetUnit().GetDamage(values[0],null,SkillType.Attack);
            }
            Debug.Log("천둥번개");
        }

        public override string ReturnInformation()
        {
            return $"시작 시 랜덤 타일 한개에 " +
                   $"{ColorText.GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>의 데미지를 입히는" +
                   $"뇌운을 시전합니다.";
        }
    }
}