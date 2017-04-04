using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingMiniGame;
using TradingSelection;

namespace Common
{
    public static class PersistentDataManager
    {
        public static TradingSelectionSceneData tradingSelectionSceneData { get; set; }
        public static TradingMiniGameSceneData tradingMiniGameSceneData { get; set; }
        public static IPlayerAssetController playerAssetController { get; set; }
    }
}
