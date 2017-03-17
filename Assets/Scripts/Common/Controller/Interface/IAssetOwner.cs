

using System.Collections.Generic;

namespace Common
{
    public interface IAssetOwner
    {
        IDictionary<CommodityType,Commodity> pendingToBuy { get; }
        IDictionary<CommodityType,Commodity> pendingToSell { get; }
        
        void Buy(IAssetOwner seller, Commodity commodity);
        void Sell(IAssetOwner buyer, Commodity commodity);
        void AddSupply(Commodity commodity);
        void AddDemand(CommodityType commodityType);

        void ClearPendingTransactions();
        void ClearPendingTransactions(CommodityType commodityType);
        void ExecutePendingTransactions();

        HashSet<CommodityType> Demands { get; }
        IDictionary<CommodityType, Commodity> Supplies { get; }
    }
}
