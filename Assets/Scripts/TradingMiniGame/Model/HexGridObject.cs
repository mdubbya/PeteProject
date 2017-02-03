
using System;
using UnityEngine;

namespace TradingMiniGame
{
    public class HexGridObject : IGridObject
    {

        GridObjectType _gridObjectType;
        public GridObjectType gridObjectType
        {
            get
            {
                return _gridObjectType;
            }

            set
            {
                _gridObjectType = value;
            }
        }

        int _pathCost;
        public int pathCost
        {
            get
            {
                return _pathCost;
            }

            set
            {
                _pathCost = value;
            }
        }
    }
}
