using System.Collections;
using System.Collections.Generic;

namespace TradingMiniGame
{
    public interface IGameGridController : IEnumerable, IEnumerable<IGridObject>
    {
        //collection related (IGameGrid is a collection of IGridObject)
        IGridObject this[GridIndex index] { get; }
        IGridObject this[int row, int column] { get; }
        GridIndex IndexOf(IGridObject gridObject);

        GridIndex start { get; set; }
        GridIndex end { get; set; }

        void BuildGrid(int rows, int columns);
        void ClearSelection();
        void ClearNeighbors(GridIndex index);
        void RemoveNeighbor(GridIndex index, GridDirection dir);
        List<GridIndex> GetNeighbors(GridIndex index);

        List<GridIndex> GetShortestPath();
        List<GridIndex> GetSelectedPath();

        bool SelectIndex(GridIndex index);

    }
}
