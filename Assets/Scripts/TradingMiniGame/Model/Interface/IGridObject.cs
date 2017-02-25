using UnityEngine;

namespace TradingMiniGame
{
    public interface IGridObject
    {
        float pathCost { get; set; }

        GameObject gameObject { get; }
    }
}