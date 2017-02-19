using System.Collections;
using System.Collections.Generic;

namespace TradingMiniGame
{
    public interface IGameGridController : IEnumerable, IEnumerable<IGridObject>
    {
        //collection related (IGameGrid should be collection of IGridObject)
        IGridObject this[GridIndex index] { get; }
        IGridObject this[int row, int column] { get; }
        GridIndex IndexOf(IGridObject gridObject);

        GridIndex start { get; set; }
        GridIndex end { get; set; }

        void BuildGrid(int rows, int columns);
        List<GridIndex> GetShortestPath();
        List<GridIndex> GetSelectedPath();
        bool SelectIndex(GridIndex index);
        void ClearSelection();
        
    }
}
