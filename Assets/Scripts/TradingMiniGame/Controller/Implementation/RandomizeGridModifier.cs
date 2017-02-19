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
        public float maxPathCost;
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
                if (_gameGridController.Count() == 0)
                {
                    throw new UnityException("Grid has no contents.");
                }
                foreach(IGridObject gridObject in _gameGridController)
                {
                    gridObject.pathCost = Random.Range(0, maxPathCost);
                    List<GridDirection> availableDirections = new List<GridDirection>(Enum.GetValues(typeof(GridDirection)).Cast<GridDirection>());
                    int numToRemove = Random.Range(0, maxNeighborsRemoved);
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
