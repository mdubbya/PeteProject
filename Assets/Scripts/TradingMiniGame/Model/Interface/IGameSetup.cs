using System.Collections.Generic;

namespace TradingMiniGame
{
    public interface IGameSetup
    {
        int rows { get; set; }
        int columns { get; set; }

        void Start();
    }
}
