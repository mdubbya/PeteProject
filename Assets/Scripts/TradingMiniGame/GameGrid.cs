using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TradingMiniGame
{
    public class GameGrid : MonoBehaviour, IEnumerable<IGridObject>
    {
        public int columns;
        public int rows;
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


        public GridIndex IndexOf(IGridObject gridObject)
        {
            return _gridObjects.FirstOrDefault(p => p.Value == gridObject).Key;
        }


        public void Start()
        {
            Populate();
            PositionObjects();
        }


        public void Populate()
        {
            _gridObjects = new Dictionary<GridIndex, IGridObject>();
            for(int row = 0; row < rows; row++)
            {
                for(int column = 0; column < columns; column++)
                {
                    GridIndex gridIndex = new GridIndex(row, column);
                    IGridObject toAdd = Instantiate(gridObjectPrefab).GetComponent<IGridObject>();

                    _gridObjects.Add(gridIndex, toAdd);
                }
            }
        }


        public void PositionObjects()
        {
            IGridObject prefabScript = gridObjectPrefab.GetComponent<IGridObject>();
            Vector3 bottomLeft = gameObject.transform.position - new Vector3(prefabScript.size.x * columns, prefabScript.size.y * rows);
            float ySpacing = prefabScript.size.y;
            float yOffset = prefabScript.size.y / 2;
            float xSpacing = prefabScript.size.x * 0.75f;
            float xOffset = prefabScript.size.x / 2;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                        
                    Vector3 position = bottomLeft + new Vector3((xSpacing * column), bottomLeft.y + (ySpacing * row),bottomLeft.z);
                    //if an uneven column number, offset to the left
                    if(column%2!=0)
                    {
                        position = new Vector3(position.x, position.y + yOffset, position.z);
                    }

                    this[row, column].MoveToPosition(position);
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