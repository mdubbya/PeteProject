using System.Collections.Generic;

namespace TradingMiniGame
{
    public interface IGridObject
    {
        float pathCost { get; set; }

        List<GridDirection> permittedTravelDirections { get; }
        List<GridIndex> neighbors { get; }

        void ClearNeighbors();

        void RemoveNeighbor(GridDirection dir);

        GridIndex this[GridDirection dir] { get; set; }
    }
}