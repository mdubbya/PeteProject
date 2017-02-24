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
        public GameObject gridObjectPrefab;
        new public Camera camera;

        private float _ySpacing;
        private float _yOffset;
        private float _xSpacing;
        private Vector3 _bottomLeft;
        private Dictionary<GameObject,GridIndex> _gridGameObjects;
        private Dictionary<GridIndex, GameObject> _gridIndices;
        private IGameGridController _gameGridController;
        

        [Inject]
        public void Initialize(IGameGridController gameGridController)
        {
            _gameGridController = gameGridController;
        }


        public void Setup(int rows, int columns)
        {
            if (_gridGameObjects != null)
            {
                _gridGameObjects.Keys.ToList().ForEach(p => Destroy(p));
            }

            _gridGameObjects = new Dictionary<GameObject, GridIndex>();
            _gridIndices = new Dictionary<GridIndex, GameObject>();
            

            Vector3 size = gridObjectPrefab.GetComponent<MeshRenderer>().bounds.size;
            _ySpacing = size.y;
            _yOffset = size.y / 2;
            _xSpacing = size.x * 0.75f;
            _bottomLeft = gameObject.transform.position;
            _bottomLeft = new Vector3(_bottomLeft.x - _xSpacing / 2 * (columns - 1), (_bottomLeft.y) - _ySpacing / 4 * (rows - 1), _bottomLeft.z);
        }


        public void SpawnGridObject(GridIndex index)
        {
            GameObject spawnedObject = Instantiate(gridObjectPrefab.gameObject);
            Vector3 position = _bottomLeft + new Vector3((_xSpacing * index.columnNumber), _bottomLeft.y + (_ySpacing * index.rowNumber));
            if (index.columnNumber % 2 != 0)
            {
                position = new Vector3(position.x, position.y + _yOffset, position.z);
            }

            spawnedObject.transform.position = position;

            _gridGameObjects.Add(spawnedObject,index);
            _gridIndices.Add(index, spawnedObject);
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