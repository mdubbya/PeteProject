using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zenject;

namespace TradingMiniGame
{
    public class GameGridController : IEnumerable, IEnumerable<IGridObject>, IGameGridController
    {
        private int _columns;
        private int _rows;

        private IGridObject _start;
        public IGridObject start
        {
            get { return _start; }
            set { _start = value; }
        }


        private IGridObject _end;
        public IGridObject end
        {
            get { return _end; }
            set { _end = value; }
        }


        private bool _pathValid;
        public bool pathValid
        {
            get { return _pathValid; }
            set { _pathValid = value; }
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


        private Dictionary<GridIndex, IGridObject> _gridObjects;
        private IGameGrid _gameGrid;
        private IFactory<IGridObject> _gridObjectFactory;

        [Inject]
        public GameGridController(IGameGrid gameGrid, IFactory<IGridObject> gridObjectFactory)
        {
            _gridObjectFactory = gridObjectFactory;
            _gameGrid = gameGrid;
            _gridObjects = new Dictionary<GridIndex, IGridObject>();
        }


        public void BuildGrid(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    GridIndex index = new GridIndex(row, column);
                    _gameGrid.SpawnGridObject(index);
                    IGridObject newGridObject = _gridObjectFactory.Create();
                    
                    foreach(GridDirection dir in newGridObject.permittedTravelDirections)
                    {
                        GridIndex neighborIndex = _indexInDirection[dir](index);
                        if(neighborIndex.columnNumber>=0 && neighborIndex.rowNumber >=0)
                        {
                            newGridObject[dir] = neighborIndex;
                        }
                    }

                    _gridObjects.Add(new GridIndex(row, column), newGridObject);
                }
            }
        }
        

        public List<GridIndex> GetShortestPath()
        {
            Dictionary<GridIndex, float> distancesFromStart = new Dictionary<GridIndex, float>();
            List<GridIndex> path = new List<GridIndex>();
            _gridObjects.Keys.ToList().ForEach(p => distancesFromStart.Add(p, float.MaxValue));
            distancesFromStart[IndexOf(start)] = 0;

            List<GridIndex> visited = new List<GridIndex>();
            visited.Add(IndexOf(start));

            List<GridIndex> unVisited = new List<GridIndex>();
            _gridObjects.Keys.ToList().ForEach(p => unVisited.Add(p));
            unVisited.Remove(IndexOf(start));
            IGridObject currentNode = start;
            
            while (currentNode != null)
            {
                List<GridIndex> currentUnvisited = (from p in currentNode.neighbors where unVisited.Contains(p) select p).ToList();
                foreach (GridIndex unvisitedNode in currentUnvisited)
                {
                    float dist = distancesFromStart[IndexOf(currentNode)] + this[unvisitedNode].pathCost;

                    if (dist < distancesFromStart[unvisitedNode])
                    {
                        distancesFromStart[unvisitedNode] = dist;
                        if(path.Contains(unvisitedNode))
                        {
                            path.Remove(unvisitedNode);
                        }
                        path.Add(IndexOf(currentNode));
                    }
                }

                visited.Add(IndexOf(currentNode));
                unVisited.Remove(IndexOf(currentNode));

                if (visited.Contains(IndexOf(end)))
                {
                    break;
                }

                currentNode = this[unVisited.OrderBy(p => distancesFromStart[p]).First()];
            }

            return path;
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
