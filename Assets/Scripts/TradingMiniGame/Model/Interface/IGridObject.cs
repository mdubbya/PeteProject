using UnityEngine;

namespace TradingMiniGame
{
    public interface IGridObject
    {
        int pathCost { get; set; }

        GameObject gameObject { get; }

        GameResources.Materials material { get; set; }
    }
}