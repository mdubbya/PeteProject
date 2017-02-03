using UnityEngine;
namespace TradingMiniGame
{
    public interface IGridObject
    {
        GridObjectType gridObjectType
        {
            get;
            set;
        }

        int pathCost
        {
            get;
            set;
        }
    }
}