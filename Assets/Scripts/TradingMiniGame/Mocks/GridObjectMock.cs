using UnityEngine;

namespace TradingMiniGame.Mocks
{
    public class GridObjectMock : MonoBehaviour, IGridObject
    {
        private Vector3 _position;

        public GridObjectType gridObjectType
        {
            get
            {
                return GridObjectType.RegularHexagon;
            }
            set
            {

            }
        }

        public Vector3 size
        {
            get
            {
                return new Vector3(1, 1, 1);
            }
        }

        public void Destroy()
        {

        }

        public void MoveToPosition(Vector3 position)
        {
            _position = position;
        }
    }
}
