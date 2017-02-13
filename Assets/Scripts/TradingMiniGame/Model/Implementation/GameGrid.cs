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
        private IGameGridController _gameGridController;

        public void Setup()
        {
            IGameSetup setup = GetComponent<IGameSetup>();
            int rows = setup.rows;
            int columns = setup.columns;

            _gridGameObjects = new Dictionary<GameObject,GridIndex>();
            Vector3 size = gridObjectPrefab.GetComponent<MeshRenderer>().bounds.size;
            _ySpacing = size.y;
            _yOffset = size.y / 2;
            _xSpacing = size.x * 0.75f;
            _bottomLeft = gameObject.transform.position;
            _bottomLeft = new Vector3(_bottomLeft.x - _xSpacing / 2 * (columns - 1), (_bottomLeft.y) - _ySpacing / 4 * (rows - 1), _bottomLeft.z);
        }


        [Inject]
        public void Initialize(IGameGridController gameGridController)
        {
            _gameGridController = gameGridController;
            Setup();
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
        }

        
        public void Update()
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit) && _gridGameObjects.ContainsKey(hit.transform.gameObject))
            {
                
            }
        }
    }
}