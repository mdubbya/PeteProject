using System.Collections.Generic;

namespace TradingSelection
{
    public class SpacePortController : ISpacePortController
    {
        private HashSet<ISpacePortController> _establishedTradeLanes;

        public SpacePortController()
        {
            _establishedTradeLanes = new HashSet<ISpacePortController>();
        }

        public void CancelTradeLane(ISpacePortController otherPort)
        {
            if (_establishedTradeLanes.Contains(otherPort))
            {
                _establishedTradeLanes.Remove(otherPort);
            }
        }

        public void EstablishTradeLane(ISpacePortController otherPort)
        {

            _establishedTradeLanes.Add(otherPort);
        }

        public bool TradeLaneEstablished(ISpacePortController otherPort)
        {
            return _establishedTradeLanes.Contains(otherPort);
        }
    }
}

