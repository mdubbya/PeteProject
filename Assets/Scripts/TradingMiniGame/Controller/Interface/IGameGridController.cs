using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingMiniGame
{
    public interface IGameGridController
    {
        //collection related (IGameGrid should be collection of IGridObject)
        IGridObject this[GridIndex index] { get; }
        IGridObject this[int row, int column] { get; }
        GridIndex IndexOf(IGridObject gridObject);

        bool pathValid { get; set; }

        GridIndex start { get; set; }
        GridIndex end { get; set; }

        void BuildGrid(int rows, int columns);
        List<GridIndex> GetShortestPath();
        List<GridIndex> GetSelectedPath();
        bool SelectIndex(GridIndex index);
        void ClearSelection();
        
    }
}
