using System;
using System.Collections.Generic;
using System.Linq;

namespace TradingMiniGame
{
    public class HexGridObject : IGridObject
    {
        private Dictionary<GridDirection, GridIndex> _neighbors;

        public HexGridObject()
        {
            _neighbors = new Dictionary<GridDirection, GridIndex>();
        }

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


        public List<GridIndex> neighbors
        {
            get
            {
                return _neighbors.Values.ToList();
            }
        }

        public GridIndex this[GridDirection dir]
        {
            get
            {
                return _neighbors[dir];
            }

            set
            {
                GridIndex neighbor;
                if (_neighbors.TryGetValue(dir, out neighbor))
                {
                    _neighbors.Remove(dir);
                }
                else
                {
                    _neighbors.Add(dir, value);
                }
            }
        }

        public void ClearNeighbors()
        {
            _neighbors.Clear();
        }

        public void RemoveNeighbor(GridDirection dir)
        {
            if(_neighbors.ContainsKey(dir))
            {
                _neighbors.Remove(dir);
            }
        }
    }
}
