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

        [Inject]
        public GameGridController(IGameGrid gameGrid)
        {
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
                    _gridObjects.Add(new GridIndex(row, column), _gameGrid.SpawnGridObject(row,column));
                }
            }
        }
    }
}
