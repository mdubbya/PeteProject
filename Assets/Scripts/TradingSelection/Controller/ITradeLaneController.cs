

namespace TradingSelection
{
    interface ITradeLaneController
    {
        ISpacePort start { get; set; }
        ISpacePort end { get; set; }
        void EstablishTradeLane();
    }
}
