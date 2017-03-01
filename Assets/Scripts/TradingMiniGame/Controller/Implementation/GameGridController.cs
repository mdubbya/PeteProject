using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zenject;

namespace TradingMiniGame
{
    public class GameGridController :  IGameGridController
    {
        
        private Dictionary<GridIndex, IGridObject> _gridObjects;
        private Dictionary<GridIndex, Dictionary<GridDirection,GridIndex>> _neighbors;
        private Stack<GridIndex> _selectedIndices;
        private IGameGrid _gameGrid;

        private GridIndex _start;
        public GridIndex start
        {
            get { return _start; }
            set { _start = value; }
        }

        private GridIndex _end;
        public GridIndex end
        {
            get { return _end; }
            set { _end = value; }
        }
        
        private int _columns;
        public int columns
        {
            get { return _columns; }
        }
        
        private int _rows;
        public int rows
        {
            get { return _rows; }
        }

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

        
        public IEnumerator<IGridObject> GetEnumerator()
        {
            var spud = _gridObjects.GetEnumerator();
            return (from p in _gridObjects select p.Value).GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return (from p in _gridObjects select p.Value).GetEnumerator();
        }


        [Inject]
        public void Init(IGameGrid gameGrid, IFactory<IGridObject> gridObjectFactory)
        {
            _gameGrid = gameGrid;
        }


        public void BuildGrid(int rows, int columns)
        {
            
            _rows = rows;
            _columns = columns;
            _gridObjects = new Dictionary<GridIndex, IGridObject>();
            _selectedIndices = new Stack<GridIndex>();
            _neighbors = new Dictionary<GridIndex, Dictionary<GridDirection, GridIndex>>();
            _gameGrid.Setup(_rows, _columns);
            _selectedIndices.Push(start);
            ClearSelection();

            if (_start==null)
            {
                _start = new GridIndex(0, 0);
            }
            if(_end==null)
            {
                _end = new GridIndex(rows-1, columns-1);
            }
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    GridIndex index = new GridIndex(row, column);
                    IGridObject newGridObject = _gameGrid.SpawnGridObject(index);
                    _neighbors.Add(index, new Dictionary<GridDirection, GridIndex>());
                    
                    foreach(GridDirection dir in Enum.GetValues(typeof(GridDirection)))
                    {
                        GridIndex neighborIndex = _indexInDirection[dir](index);
                        if(neighborIndex.columnNumber>=0 && neighborIndex.rowNumber >=0 &&
                            neighborIndex.columnNumber< _columns && neighborIndex.rowNumber < _rows)
                        {
                            _neighbors[index][dir] = neighborIndex;
                        }
                    }

                    _gridObjects.Add(new GridIndex(row, column), newGridObject);
                }
            }
        }


        public List<GridIndex> GetShortestPath()
        {
            Dictionary<GridIndex, float> distancesFromStart = new Dictionary<GridIndex, float>();
            Dictionary<GridIndex, GridIndex> path = new Dictionary<GridIndex, GridIndex>();
            _gridObjects.Keys.ToList().ForEach(p => path.Add(p, new GridIndex(-1,-1)));

            _gridObjects.Keys.ToList().ForEach(p => distancesFromStart.Add(p, float.MaxValue));
            distancesFromStart[_start] = 0;
            
            List<GridIndex> unVisited = new List<GridIndex>();
            _gridObjects.Keys.ToList().ForEach(p => unVisited.Add(p));
            
            while (unVisited.Count > 0)
            {
                GridIndex next = unVisited.OrderBy(p => distancesFromStart[p]).First();

                if (next == _end)
                {
                    break;
                }
                unVisited.Remove(next);

                foreach (GridIndex neighborIndex in _neighbors[next].Values)
                {
                    float thisDistance = distancesFromStart[next] + this[neighborIndex].pathCost;
                    if (thisDistance < distancesFromStart[neighborIndex])
                    {
                        distancesFromStart[neighborIndex] = thisDistance;
                        path[neighborIndex] = next;
                    }
                }
            }

            Stack<GridIndex> s = new Stack<GridIndex>();
            GridIndex u = _end;
            while (true)
            {
                if (u != new GridIndex(-1,-1))
                {
                    s.Push(u);
                    u = path[u];
                }
                else break;
            }

            float shortestDist = distancesFromStart[_end];
            if (s.Count > 1)
            {
                return s.ToList();
            }
            else
            {
                return null;
            }
        }


        public bool SelectIndex(GridIndex index)
        {
            bool result = false;
            if (_selectedIndices.Contains(index))
            {
                result = true;
                UnwindSelectionToIndex(index);
            }
            else
            {
                if (_neighbors[_selectedIndices.Peek()].Values.Contains(index))
                {
                    result = true;
                    _selectedIndices.Push(index);
                }
            }
            return result;
        }


        private void UnwindSelectionToIndex(GridIndex index)
        {
            while(_selectedIndices.Peek() != index)
            {
               _selectedIndices.Pop();
            }
        }


        public void ClearSelection()
        {
            UnwindSelectionToIndex(_start);
        }

        public List<GridIndex> GetSelectedPath()
        {
            List<GridIndex> returnVal = _selectedIndices.ToList();
            returnVal.Reverse();
            return returnVal;
        }

        public void ClearNeighbors(GridIndex index)
        {
            _neighbors[index].Clear();
        }

        public void RemoveNeighbor(GridIndex index, GridDirection dir)
        {
            _neighbors[index].Remove(dir);
        }

        public List<GridDirection> GetAdjacentDisconnected(GridIndex index)
        {
            var enumValues = Enum.GetValues(typeof(GridDirection)).Cast<GridDirection>().ToList();
            return enumValues.Where(p => !_neighbors[index].Keys.Contains(p)).ToList();
        }


        public List<GridDirection> GetAdjacentConnected(GridIndex index)
        {
            return _neighbors[index].Keys.ToList();
        }

        public GridIndex GetRelativeIndex(GridIndex index, GridDirection dir)
        {
            return _indexInDirection[dir](index);
        }

        private static Dictionary<GridDirection, Func<GridIndex, GridIndex>> _indexInDirection = new Dictionary<GridDirection, Func<GridIndex, GridIndex>>
        {
            { GridDirection.N, new Func<GridIndex,
                GridIndex>(p => new GridIndex(++p.rowNumber,p.columnNumber)) },
            { GridDirection.S, new Func<GridIndex,
                GridIndex>(p => new GridIndex(--p.rowNumber,p.columnNumber)) },
            { GridDirection.NE, new Func<GridIndex,
                GridIndex>(p => new GridIndex(p.columnNumber%2==0? p.rowNumber: ++p.rowNumber, ++p.columnNumber)) },
            { GridDirection.NW, new Func<GridIndex, 
                GridIndex>(p => new GridIndex(p.columnNumber%2==0? p.rowNumber: ++p.rowNumber, --p.columnNumber)) },
            { GridDirection.SE, new Func<GridIndex, 
                GridIndex>(p => new GridIndex(p.columnNumber%2==0? --p.rowNumber: p.rowNumber, ++p.columnNumber)) },
            { GridDirection.SW, new Func<GridIndex, 
                GridIndex>(p => new GridIndex(p.columnNumber%2==0? --p.rowNumber: p.rowNumber, --p.columnNumber)) },
        };
    }
}
