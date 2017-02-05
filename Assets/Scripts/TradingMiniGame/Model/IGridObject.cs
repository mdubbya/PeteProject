using System.Collections.Generic;

namespace TradingMiniGame
{
    public interface IGridObject
    {
        float pathCost { get; set; }

        List<GridDirection> permittedTravelDirections { get; }
        List<GridIndex> neighbors { get; }

        GridIndex this[GridDirection dir] { get; set; }
    }
}