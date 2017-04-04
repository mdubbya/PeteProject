using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingSelection
{
    public class TradingSelectionSceneData
    {
        public HashSet<ISpacePort> spacePorts;

        public TradingSelectionSceneData()
        {
            spacePorts = new HashSet<ISpacePort>();
        }
    }
}
