using UnityEngine;

namespace Map
{
    public class StagePoint : MonoBehaviour
    {
        private MapState mapState;

        public void Init(MapState mapState)
        {
            this.mapState = mapState;
        }
    }
}