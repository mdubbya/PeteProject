using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TradingMiniGame
{
    public class GridObject :  MonoBehaviour, IGridObject 
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
