using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{ 
    public class SpacePortAssetController : IAssetController
    {
        private Dictionary<CommodityType, Commodity> _pendingToBuy;
        public IDictionary<CommodityType, Commodity> pendingToBuy
        {
            get
            {
                return _pendingToBuy;
            }
        }

        private Dictionary<CommodityType, Commodity> _pendingToSell;
        public IDictionary<CommodityType, Commodity> pendingToSell
        {
            get
            {
                return _pendingToSell;
            }
        }

        private HashSet<CommodityType> _demands;
        public HashSet<CommodityType> Demands
        {
            get
            {
                return _demands;
            }
        }

        private Dictionary<CommodityType, Commodity> _supplies;
        public IDictionary<CommodityType, Commodity> Supplies
        {
            get
            {
                return _supplies;
            }
        }
        

        public SpacePortAssetController()
        {
            _supplies = new Dictionary<CommodityType, Commodity>();
            _demands = new HashSet<CommodityType>();
            _pendingToBuy = new Dictionary<CommodityType, Commodity>();
            _pendingToSell = new Dictionary<CommodityType, Commodity>();
        }


        public void ClearPendingTransactions()
        {
            _pendingToBuy.Clear();
        }


        public void ExecutePendingTransactions()
        {
            _pendingToBuy.ToList().ForEach(p => _supplies.Add(p.Key,p.Value));
            _pendingToBuy.ToList().ForEach(p => _demands.Remove(p.Key));
            _pendingToBuy.Clear();

            _pendingToSell.ToList().ForEach(p => _supplies.Remove(p.Key));
            _pendingToSell.Clear();
        }
        

        public void Buy(IAssetController seller, Commodity commodity)
        {
            if (_demands.Contains(commodity.commodityType) && !_pendingToBuy.ContainsKey(commodity.commodityType))
            {
                _pendingToBuy.Add(commodity.commodityType, commodity);
            }
        }


        public void Sell(IAssetController buyer, Commodity commodity)
        {
            if (buyer.Demands.Contains(commodity.commodityType) && !_pendingToSell.ContainsKey(commodity.commodityType))
            {
                _pendingToSell.Add(commodity.commodityType, commodity);
            }
        }


        public void ClearPendingTransactions(CommodityType commodityType)
        {
            _pendingToBuy.Remove(commodityType);
        }
        

        public void AddSupply(Commodity commodity)
        {
            _supplies.Add(commodity.commodityType,commodity);
        }


        public void AddDemand(CommodityType commodityType)
        {
            _demands.Add(commodityType);
        }


        private float GetModifiedCost(Commodity commodity)
        {
            return commodity.value;
        }
    }
}
