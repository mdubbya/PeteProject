

using System.Collections.Generic;

namespace Common
{
    public interface IAssetController
    {
        IDictionary<CommodityType,Commodity> pendingToBuy { get; }
        IDictionary<CommodityType,Commodity> pendingToSell { get; }
        
        void Buy(IAssetController seller, Commodity commodity);
        void Sell(IAssetController buyer, Commodity commodity);
        void AddSupply(Commodity commodity);
        void AddDemand(CommodityType commodityType);

        void ClearPendingTransactions();
        void ClearPendingTransactions(CommodityType commodityType);
        void ExecutePendingTransactions();

        HashSet<CommodityType> Demands { get; }
        IDictionary<CommodityType, Commodity> Supplies { get; }
    }
}
