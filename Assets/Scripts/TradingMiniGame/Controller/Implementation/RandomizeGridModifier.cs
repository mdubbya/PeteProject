using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace TradingMiniGame
{
    public class RandomizeGridModifier : MonoBehaviour, IGameGridSetupModifier
    {
        private IGameGridController _gameGridController;
        public int maxPathCost;
        public int maxNeighborsRemoved;

        [Inject]
        public void Initialize(IGameGridController gameGridController)
        {
            _gameGridController = gameGridController;
        }

        public void Modify()
        {
            bool success = false;
            
            while (!success)
            {
                GridIndex randomStart = new GridIndex(Random.Range(0, _gameGridController.rows - 1), Random.Range(0, _gameGridController.columns - 1));
                GridIndex randomEnd = new GridIndex(Random.Range(0, _gameGridController.rows - 1), Random.Range(0, _gameGridController.columns - 1));
                _gameGridController.start = randomStart;
                _gameGridController.end = randomEnd;
                _gameGridController.BuildGrid(_gameGridController.rows,_gameGridController.columns);

                if (_gameGridController.Count() == 0)
                {
                    throw new UnityException("Grid has no contents.");
                }
                foreach(IGridObject gridObject in _gameGridController)
                {
                    gridObject.pathCost = Random.Range(0, maxPathCost);
                    List<GridDirection> availableDirections = new List<GridDirection>(Enum.GetValues(typeof(GridDirection)).Cast<GridDirection>());
                    int numToRemove = Math.Min( Random.Range(0, maxNeighborsRemoved), Enum.GetValues(typeof(GridDirection)).Length);
                    while (numToRemove > 0)
                    {
                        int removeIndex = Random.Range(0, availableDirections.Count);
                        _gameGridController.RemoveNeighbor(_gameGridController.IndexOf(gridObject),availableDirections[removeIndex]);
                        availableDirections.RemoveAt(removeIndex);
                        numToRemove--;
                    }
                }
                success = _gameGridController.GetShortestPath() != null;
            }
        }
    }
}
