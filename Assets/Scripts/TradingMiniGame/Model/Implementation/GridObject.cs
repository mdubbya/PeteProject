using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;
using System.Collections.Generic;
using System;

namespace TradingMiniGame
{
    public class GridObject :  MonoBehaviour, IGridObject 
    {
        public Image connectionN;
        public Image connectionNE;
        public Image connectionSE;
        public Image connectionS;
        public Image connectionSW;
        public Image connectionNW;

        Text _text;
        private IGameGridController _gameGridController;
        private Dictionary<GridDirection, Image> _connectionImages;

        [Inject]
        public void Initialize(IGameGridController controller)
        {
            _gameGridController = controller;
            _text = GetComponentInChildren<Text>();
            _connectionImages = new Dictionary<GridDirection, Image>()
            {
                { GridDirection.N, connectionN },
                { GridDirection.NE, connectionNE },
                { GridDirection.SE, connectionSE },
                { GridDirection.S, connectionS },
                { GridDirection.SW, connectionSW },
                { GridDirection.NW, connectionNW }
            };
        }

        int _pathCost;
        public int pathCost
        {
            get
            {
                return _pathCost;
            }

            set
            {
                _pathCost = value;
                _text.text = _pathCost.ToString();
            }
        }

        private GameResources.Materials _material;
        public GameResources.Materials material
        {
            get
            {
                return _material;
            }

            set
            {
                _material = value;
                GetComponent<Renderer>().material = GameResources.Load(value);
            }
        }

        public void Update()
        {
            List<GridDirection> disconnected = _gameGridController.GetAdjacentDisconnected(_gameGridController.IndexOf(this));
            List<GridDirection> connected = _gameGridController.GetAdjacentConnected(_gameGridController.IndexOf(this));
            connected.ForEach(p => Destroy( _connectionImages[p]));
        }

        
    }
}
