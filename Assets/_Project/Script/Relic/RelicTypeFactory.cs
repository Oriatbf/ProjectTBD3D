using System;

namespace _Project.Script.Relic
{
    public static class RelicTypeFactory
    {
        public enum RelicType
        {
            Thunder,
            DamageBuff
        }

        public static RelicEffect CreateInstance(RelicType relicType)
        {
            return relicType switch
            {
                RelicType.Thunder => new Thunder(),
                RelicType.DamageBuff => new DamageBuff(),

                _ => throw new ArgumentOutOfRangeException(nameof(relicType), relicType, null)
            };
        }
    }
}