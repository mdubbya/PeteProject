using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zenject;

namespace TradingSelection
{
    public class SpacePort : MonoBehaviour, ISpacePort
    {
        private ISpacePortController _controller;
        public ISpacePortController controller
        {
            get
            {
                return _controller;
            }
        }

        [Inject]
        public void Initialize(ISpacePortController controller)
        {
            _controller = controller;
        }
    }
}
