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
        private Dictionary<GameObject,GridIndex> _gridGameObjects;
        private Dictionary<GridIndex, GameObject> _gridIndices;
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
            if (_gridGameObjects != null)
            {
                _gridGameObjects.Keys.ToList().ForEach(p => Destroy(p));
            }

            _gridGameObjects = new Dictionary<GameObject, GridIndex>();
            _gridIndices = new Dictionary<GridIndex, GameObject>();
            
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

            _gridGameObjects.Add(spawnedObject.gameObject,index);
            _gridIndices.Add(index, spawnedObject.gameObject);

            return spawnedObject;
        }

        
        public void Update()
        {
            if(Input.GetMouseButton(1))
            {
                List<GameObject> selected = _gameGridController.GetShortestPath().Select(p => _gridIndices[p]).ToList();
                selected.ForEach(p => p.GetComponent<Renderer>().material = GameResources.Load(GameResources.Materials.Red));
            }

            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (_gridGameObjects.ContainsKey(hit.transform.gameObject))
                    {
                        _gameGridController.SelectIndex(_gridGameObjects[hit.transform.gameObject]);
                        List<GameObject> selected = _gameGridController.GetSelectedPath().Select(p => _gridIndices[p]).ToList();
                        selected.ForEach(p => p.GetComponent<Renderer>().material = GameResources.Load(GameResources.Materials.Green));
                        foreach(GameObject gObject in _gridGameObjects.Keys)
                        {
                            if(!selected.Contains(gObject))
                            {
                                gObject.GetComponent<Renderer>().material = GameResources.Load(GameResources.Materials.Blue);
                            }
                        }
                    }
                }
            }
        }
    }
}