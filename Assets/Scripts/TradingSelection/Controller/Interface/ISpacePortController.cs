using Common;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public interface ISpacePortController
    {
        void EstablishTradeLane();
        bool TradeLaneEstablished(ISpacePortController otherPort);
    }
}
