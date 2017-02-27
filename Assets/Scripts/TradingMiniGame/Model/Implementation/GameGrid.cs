using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace TradingMiniGame
{
    public class GameGrid : MonoBehaviour, IGameGrid
    {
        new public Camera camera;

        private float _ySpacing;
        private float _yOffset;
        private float _xSpacing;
        private Vector3 _bottomLeft;
        private Dictionary<GridIndex, IGridObject> _gridObjects;
        private IGameGridController _gameGridController;
        private Vector3 _tileSize;
        private IFactory<IGridObject> _gridObjectFactory;

        [Inject]
        public void Initialize(IGameGridController gameGridController, IFactory<IGridObject> gridObjectFactory, Vector3 tileSize)
        {
            _gameGridController = gameGridController;
            _gridObjectFactory = gridObjectFactory;
            _tileSize = tileSize;
        }


        public void Setup(int rows, int columns)
        {
            if (_gridObjects != null)
            {
                _gridObjects.Values.ToList().ForEach(p => Destroy(p.gameObject));
            }
            
            _gridObjects = new Dictionary<GridIndex, IGridObject>();
            
            _ySpacing = _tileSize.y;
            _yOffset = _tileSize.y / 2;
            _xSpacing = _tileSize.x * 0.75f;
            _bottomLeft = gameObject.transform.position;
            _bottomLeft = new Vector3(_bottomLeft.x - _xSpacing / 2 * (columns - 1), (_bottomLeft.y) - _ySpacing / 4 * (rows - 1), _bottomLeft.z);
        }


        public IGridObject SpawnGridObject(GridIndex index)
        {
            IGridObject spawnedObject = _gridObjectFactory.Create();
            Vector3 position = _bottomLeft + new Vector3((_xSpacing * index.columnNumber), _bottomLeft.y + (_ySpacing * index.rowNumber));
            if (index.columnNumber % 2 != 0)
            {
                position = new Vector3(position.x, position.y + _yOffset, position.z);
            }

            spawnedObject.gameObject.transform.position = position;
            
            _gridObjects.Add(index, spawnedObject);

            return spawnedObject;
        }

        
        public void Update()
        {
            if(Input.GetMouseButton(1))
            {
                _gameGridController.GetShortestPath().Where
                    (p => _gameGridController.start != p && _gameGridController.end != p).Select
                    (p => _gameGridController[p]).ToList().ForEach
                    (p => p.material = GameResources.Materials.Red);
            }

            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (_gridObjects.Select(p => p.Value.gameObject).Contains(hit.transform.gameObject))
                    {
                        _gameGridController.SelectIndex(_gridObjects.First(p => p.Value.gameObject==hit.transform.gameObject).Key);
                        List<IGridObject> selected = _gameGridController.GetSelectedPath().Where
                            (p => _gameGridController.start!=p && _gameGridController.end != p).Select(p => _gridObjects[p]).ToList();
                        selected.ForEach(p => p.material = GameResources.Materials.Green);
                        foreach(IGridObject gObject in _gridObjects.Select(p => p.Value).Where(p => !selected.Contains(p)))
                        {
                            if(!selected.Contains(gObject))
                            {
                                gObject.material = GameResources.Materials.Blue;
                            }
                        }
                    }
                }
            }
            _gridObjects[_gameGridController.start].material = GameResources.Materials.Green;
            _gridObjects[_gameGridController.end].material = GameResources.Materials.Yellow;
        }
    }
}