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
        private int _columns;
        private int _rows;
        private Dictionary<GridIndex, IGridObject> _gridObjects;
        private Dictionary<GridIndex, Dictionary<GridDirection,GridIndex>> _neighbors;
        private Stack<GridIndex> _selectedIndices;
        private IGameGrid _gameGrid;
        private IFactory<IGridObject> _gridObjectFactory;

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
            _gridObjectFactory = gridObjectFactory;
            _gameGrid = gameGrid;
            _gridObjects = new Dictionary<GridIndex, IGridObject>();
            _selectedIndices = new Stack<GridIndex>();
            _neighbors = new Dictionary<GridIndex, Dictionary<GridDirection, GridIndex>>();
        }


        public void BuildGrid(int rows, int columns)
        {
            _gridObjects.Clear();
            _selectedIndices.Clear();
            _selectedIndices.Push(start);
            _neighbors.Clear();
            _rows = rows;
            _columns = columns;
            if(_start==null)
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
                    _gameGrid.SpawnGridObject(index);
                    IGridObject newGridObject = _gridObjectFactory.Create();
                    _neighbors.Add(index, new Dictionary<GridDirection, GridIndex>());
                    
                    foreach(GridDirection dir in Enum.GetValues(typeof(GridDirection)))
                    {
                        GridIndex neighborIndex = _indexInDirection[dir](index);
                        if(neighborIndex.columnNumber>=0 && neighborIndex.rowNumber >=0)
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
            Dictionary<GridIndex,GridIndex> path = new Dictionary<GridIndex, GridIndex>();
            _gridObjects.Keys.ToList().ForEach(p => distancesFromStart.Add(p, float.MaxValue));
            distancesFromStart[_start] = 0;

            List<GridIndex> visited = new List<GridIndex>();
            visited.Add(_start);

            List<GridIndex> unVisited = new List<GridIndex>();
            _gridObjects.Keys.ToList().ForEach(p => unVisited.Add(p));
            unVisited.Remove(_start);
            IGridObject currentNode = this[_start];
            
            while (currentNode != null)
            {
                List<GridIndex> currentUnvisited = (from p in _neighbors[IndexOf(currentNode)].Values where unVisited.Contains(p) select p).ToList();
                foreach (GridIndex unvisitedNode in currentUnvisited)
                {
                    float dist = distancesFromStart[IndexOf(currentNode)] + this[unvisitedNode].pathCost;

                    if (dist < distancesFromStart[unvisitedNode])
                    {
                        distancesFromStart[unvisitedNode] = dist;
                        if(path.ContainsKey(unvisitedNode))
                        {
                            path.Remove(unvisitedNode);
                        }
                        path.Add(unvisitedNode,IndexOf(currentNode));
                    }
                }

                visited.Add(IndexOf(currentNode));
                unVisited.Remove(IndexOf(currentNode));

                if (visited.Contains(_end))
                {
                    break;
                }

                currentNode = this[unVisited.OrderBy(p => distancesFromStart[p]).First()];
            }

            //unwind
            List<GridIndex> returnedPath = new List<GridIndex>();
            GridIndex endIndex = _end;
            GridIndex next;
            bool pathPossible = path.TryGetValue(endIndex, out next);
            if (pathPossible)
            {
                returnedPath.Add(endIndex);
                while (next != _start)
                {
                    returnedPath.Insert(0, next);
                    next = path[next];
                }
            }
            else
            {
                returnedPath = null;
            }
            return returnedPath;
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
                if (_neighbors[index].Values.Contains(_selectedIndices.Peek()))
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

        public List<GridIndex> GetNeighbors(GridIndex index)
        {
            return _neighbors[index].Values.ToList();
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
