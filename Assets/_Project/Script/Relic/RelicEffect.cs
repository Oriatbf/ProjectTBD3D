using System.Collections.Generic;

namespace _Project.Script.Relic
{
    public abstract class RelicEffect
    {
        protected List<int> values = new List<int>();
        public abstract void Excute();

        public void Init(List<int> _values)
        {
            values = _values;
        }
    }
}