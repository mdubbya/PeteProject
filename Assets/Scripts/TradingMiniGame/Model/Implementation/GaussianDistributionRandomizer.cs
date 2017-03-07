using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace TradingMiniGame
{
    public class GaussianDistributionRandomizer : MonoBehaviour
    {
        private IGameGridController _gameGridController;
        public int maxPathCost;
        [Range(0,5)]
        public int maxNeighborsRemoved;
        [Range(0, 5)]
        public int minNeighborsRemoved;
        [Range(0,1)]
        public float mean;
        [Range(0,1)]
        public float standardDeviation;
        [Range(float.Epsilon,1)]
        public float neighborWeight;
        [Range(0, 1)]
        public float minStartEndDistancePercentage;

        [Inject]
        public void Initialize(IGameGridController gameGridController)
        {
            _gameGridController = gameGridController;
            if (_gameGridController.rows > 0 && _gameGridController.columns > 0)
            {
                Modify();
            }
        }

        public void Modify()
        {
            bool success = false;

            bool startAndEndSeparated = false;
            float gridLinearSize = Vector2.Distance(new Vector2(0, 0), new Vector2(_gameGridController.rows - 1, _gameGridController.columns - 1));
            GridIndex randomStart = new GridIndex(0, 0);
            GridIndex randomEnd = new GridIndex(0, 0);
            while (!startAndEndSeparated)
            {
                randomStart = new GridIndex(Random.Range(0, _gameGridController.rows - 1), Random.Range(0, _gameGridController.columns - 1));
                randomEnd = new GridIndex(Random.Range(0, _gameGridController.rows - 1), Random.Range(0, _gameGridController.columns - 1));
                startAndEndSeparated = Vector2.Distance(new Vector2(randomStart.rowNumber, randomStart.columnNumber),
                    new Vector2(randomEnd.rowNumber, randomEnd.columnNumber))>(minStartEndDistancePercentage * gridLinearSize);
            }
            _gameGridController.start = randomStart;
            _gameGridController.end = randomEnd;

            while (!success)
            {
                
                _gameGridController.BuildGrid(_gameGridController.rows,_gameGridController.columns);

                if (_gameGridController.Count() == 0)
                {
                    throw new UnityException("Grid has no contents.");
                }
                foreach(IGridObject gridObject in _gameGridController)
                {
                    List<GridDirection> availableDirections = new List<GridDirection>(Enum.GetValues(typeof(GridDirection)).Cast<GridDirection>());
                    int numToRemove = Math.Min( Random.Range(minNeighborsRemoved, maxNeighborsRemoved), Enum.GetValues(typeof(GridDirection)).Length);
                    while (numToRemove > 0)
                    {
                        int removeIndex = Random.Range(0, availableDirections.Count);
                        _gameGridController.RemoveNeighbor(_gameGridController.IndexOf(gridObject),availableDirections[removeIndex]);
                        availableDirections.RemoveAt(removeIndex);
                        numToRemove--;
                    }

                    gridObject.pathCost = (int)(Math.Round(maxPathCost * Mathf.Clamp(BoxMuller() - (neighborWeight*(numToRemove/6)) ,0,1)));
                }
                success = _gameGridController.GetShortestPath() != null;
            }
        }

        private float _y2;
        private bool _useLast = false;

        private float BoxMuller()  
        {                       
            float x1, x2, w, y1;

            if (_useLast)               
            {
                y1 = _y2;
                _useLast = false;
            }
            else
            {
                x1 = 2f * Random.Range(0f, 1f) - 1f;
                x2 = 2f * Random.Range(0f, 1f) - 1f;
                w = x1 * x1 + x2 * x2;

                while (w >= 1.0)
                {
                    x1 = 2f * Random.Range(0f,1f) - 1f;
                    x2 = 2f * Random.Range(0f, 1f) - 1f;
                    w = x1 * x1 + x2 * x2;
                } 

                w = Mathf.Sqrt((-2f * Mathf.Log(w)) / w);
                y1 = x1 * w;
                _y2 = x2 * w;
                _useLast = true;
            }

            return (mean + y1 * standardDeviation);
        }

    }
}
