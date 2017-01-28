using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TradingMiniGame
{
    public class GameGrid : MonoBehaviour, IEnumerable<IGridObject>
    {
        public int columns { get; set; }
        public int rows { get; set; }
        public GameObject gridObjectPrefab;

        private Dictionary<GridIndex, IGridObject> _gridObjects;
        
        public IGridObject this[GridIndex index]
        {
            get
            {
                return _gridObjects[index];
            }
        }


        public IGridObject this[int row, int column]
        {
            get
            {
                return _gridObjects[new GridIndex(row, column)];
            }
        }


        public void Populate()
        {
            _gridObjects = new Dictionary<GridIndex, IGridObject>();
            for(int row = 0; row < rows; row++)
            {
                for(int column = 0; column < columns; column++)
                {
                    GridIndex gridIndex = new GridIndex(row, column);
                    var obj = Instantiate(gridObjectPrefab);
                    var gObj = obj.GetComponent<IGridObject>();
                    _gridObjects.Add(gridIndex, obj.GetComponent<IGridObject>());
                }
            }
        }

        public IEnumerator<IGridObject> GetEnumerator()
        {
            var spud = _gridObjects.GetEnumerator();
            return (from p in _gridObjects select p.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (from p in _gridObjects select p.Value).GetEnumerator();
        }
    }
}