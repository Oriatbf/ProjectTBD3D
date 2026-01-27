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
        
        /// <summary>
        /// 각 스킬별 설명을 적어서 리턴하는 코드
        /// </summary>
        public abstract string ReturnInformation();
    }
}