using System;
using System.Collections.Generic;
using System.Linq;

namespace TradingMiniGame
{
    public class GridObject : IGridObject
    {
        float _pathCost;
        public float pathCost
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
