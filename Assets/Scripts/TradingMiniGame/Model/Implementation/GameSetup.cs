using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace TradingMiniGame
{
    public class GameSetup : MonoBehaviour, IGameSetup
    {
        private IGameGridController _gameGridController;

        [SerializeField]
        private int _rows;
        public int rows
        {
            get { return _rows; }
            set { _rows = value; }
        }


        [SerializeField]
        private int _columns;
        public int columns
        {
            get { return _columns; }
            set { _columns = value; }
        }


        [Inject]
        public void Initialize(IGameGridController gameGridController)
        {
            _gameGridController = gameGridController;
        }


        public void Start()
        {
            _gameGridController.start = new GridIndex(0, 0);
            _gameGridController.end = new GridIndex(5, 5);
            _gameGridController.BuildGrid(rows, columns);
            foreach(IGameGridSetupModifier modifier in GetComponents<IGameGridSetupModifier>())
            {
                modifier.Modify();
            }
        }
    }
}
