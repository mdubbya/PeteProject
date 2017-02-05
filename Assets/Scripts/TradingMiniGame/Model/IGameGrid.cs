using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingMiniGame
{
    public interface IGameGrid
    {
        int rows { get; set; }
        int columns { get; set; }
        void SpawnGridObject(GridIndex index);
    }
}
