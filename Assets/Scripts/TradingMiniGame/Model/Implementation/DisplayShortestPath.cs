using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;

namespace TradingMiniGame
{ 
    class DisplayShortestPath : MonoBehaviour
    {
        private Text _text;
        private IGameGridController _controller;

        [Inject]
        public void Initialize(IGameGridController controller)
        {
            _controller = controller;
            _text = GetComponent<Text>();
        }

        public void Update()
        {
            _text.text = _controller.GetShortestPath().Select(p => _controller[p].pathCost).Sum().ToString();
        }
    }
}
