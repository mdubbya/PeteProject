using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingMiniGame
{
    public interface IGameGrid
    {
        IGridObject SpawnGridObject(GridIndex index);

        void Setup(int rows, int columns);
    }
}
