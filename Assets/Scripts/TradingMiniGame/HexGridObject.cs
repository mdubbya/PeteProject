
using System;
using UnityEngine;

namespace TradingMiniGame
{
    public class HexGridObject : MonoBehaviour, IGridObject
    {
        public GridObjectType gridObjectType
        {
            get
            {
                return GridObjectType.RegularHexagon;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Vector3 size
        {
            get
            {
                return gameObject.GetComponent<MeshRenderer>().bounds.size;
            }
        }

        public void Destroy()
        {
            Destroy();
        }

        public void MoveToPosition(Vector3 position)
        {
            gameObject.transform.position = position;
        }
    }
}
