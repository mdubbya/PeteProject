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
        
        private float _ySpacing;
        private float _yOffset;
        private float _xSpacing;
        private Vector3 _bottomLeft;
        private Dictionary<GridIndex,GameObject> _gridGameObjects;
        private IGameGridController _gameGridController;


        public void Start()
        {
            _gridGameObjects = new Dictionary<GridIndex, GameObject>();
            Vector3 size = gridObjectPrefab.GetComponent<MeshRenderer>().bounds.size;
            _ySpacing = size.y;
            _yOffset = size.y / 2;
            _xSpacing = size.x * 0.75f;
            _bottomLeft = gameObject.transform.position;
            _bottomLeft = new Vector3(_bottomLeft.x - _xSpacing / 2 * (_columns - 1), (_bottomLeft.y) - _ySpacing / 4 * (_rows - 1), _bottomLeft.z);
            _gameGridController.BuildGrid(rows, columns);
        }


        [Inject]
        public void Initialize(IGameGridController gameGridController)
        {
            _gameGridController = gameGridController;
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

            _gridGameObjects.Add(index, spawnedObject);
        }
    }
}