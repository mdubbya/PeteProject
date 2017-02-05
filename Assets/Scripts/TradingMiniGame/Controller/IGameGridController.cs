using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingMiniGame
{
    public interface IGameGridController
    {
        IGridObject this[GridIndex index] { get; }
        IGridObject this[int row, int column] { get; }

        IGridObject end { get; set; }
        bool pathValid { get; set; }
        IGridObject start { get; set; }

        void BuildGrid(int rows, int columns);
        List<GridIndex> GetShortestPath();
        GridIndex IndexOf(IGridObject gridObject);
    }
}
