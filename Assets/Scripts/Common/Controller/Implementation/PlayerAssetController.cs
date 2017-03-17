using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common
{
    public class PlayerAssetController : MonoBehaviour, IPlayerAssetController
    {
        public float credits
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public HashSet<CommodityType> Demands
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDictionary<CommodityType, Commodity> pendingToBuy
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDictionary<CommodityType, Commodity> Supplies
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDictionary<CommodityType, Commodity> pendingToSell
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void AddDemand(CommodityType commodityType)
        {
            throw new NotImplementedException();
        }

        public void AddSupply(Commodity commodity)
        {
            throw new NotImplementedException();
        }

        public void Buy(IAssetOwner seller, Commodity commodity)
        {
            throw new NotImplementedException();
        }

        public void ClearPendingTransactions(CommodityType commodityType)
        {
            throw new NotImplementedException();
        }

        public void ClearPendingTransactions()
        {
            throw new NotImplementedException();
        }

        public void ExecutePendingTransactions()
        {
            throw new NotImplementedException();
        }

        public void Sell(IAssetOwner buyer, Commodity commodity)
        {
            throw new NotImplementedException();
        }
    }
}
