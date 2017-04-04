using Common;
using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using TradingSelection;

[TestFixture]
public class SpacePortAssetControllerTests
{
    IAssetController otherPartyLocation;
    IAssetController controllerLocation;
    IAssetController otherParty;
    SpacePortAssetController controller;

    [SetUp]
    public void Setup()
    {
        otherPartyLocation = Substitute.For<IAssetController>();
        controllerLocation = Substitute.For<IAssetController>();
        otherParty = Substitute.For<IAssetController>();
        controller = new SpacePortAssetController();
    }


    [Test]
    public void AddSupplyTest()
    {
        Commodity commodity = new Commodity(CommodityType.Diamonds, 5, controllerLocation);
        controller.AddSupply(commodity);
        Assert.AreEqual(commodity, controller.Supplies[CommodityType.Diamonds]);
    }
    

    [Test]
    public void AddDemandTest()
    {
        controller.AddDemand(CommodityType.Diamonds);
        Assert.AreEqual(CommodityType.Diamonds, controller.Demands.First());
    }
    

    [Test]
    public void BuyTest()
    {
        Dictionary<CommodityType, Commodity> otherPartySupply = 
            new Dictionary<CommodityType,Commodity>() { { CommodityType.Diamonds, new Commodity(CommodityType.Diamonds, 5, otherPartyLocation) } };
        otherParty.Supplies.Returns(otherPartySupply);
        controller.AddSupply(new Commodity(CommodityType.Gold, 5, controllerLocation));
        otherParty.Demands.Returns(new HashSet<CommodityType>() { CommodityType.Gold });
        controller.AddDemand(CommodityType.Diamonds);

        controller.Buy(otherParty, otherPartySupply[CommodityType.Diamonds]);

        Assert.AreEqual(1, controller.pendingToBuy.Count());
        Assert.AreEqual(otherPartySupply[CommodityType.Diamonds], controller.pendingToBuy[CommodityType.Diamonds]);
    }


    [Test]
    public void SellTest()
    {
        Dictionary<CommodityType, Commodity> otherPartySupply =
            new Dictionary<CommodityType, Commodity>() { { CommodityType.Diamonds, new Commodity(CommodityType.Diamonds, 5, otherPartyLocation) } };
        otherParty.Supplies.Returns(otherPartySupply);
        controller.AddSupply(new Commodity(CommodityType.Gold, 5, controllerLocation));
        otherParty.Demands.Returns(new HashSet<CommodityType>() { CommodityType.Gold });
        controller.AddDemand(CommodityType.Diamonds);

        controller.Sell(otherParty, controller.Supplies[CommodityType.Gold]);

        Assert.AreEqual(1, controller.pendingToSell.Count());
        Assert.AreEqual(controller.Supplies[CommodityType.Gold], controller.pendingToSell[CommodityType.Gold]);
    }


    [Test]
    public void ClearPendingTransactionTest()
    {
        Dictionary<CommodityType, Commodity> otherPartySupply =
            new Dictionary<CommodityType, Commodity>() { { CommodityType.Diamonds, new Commodity(CommodityType.Diamonds, 5, otherPartyLocation) } };
        otherParty.Supplies.Returns(otherPartySupply);
        controller.AddSupply(new Commodity(CommodityType.Gold, 5, controllerLocation));
        otherParty.Demands.Returns(new HashSet<CommodityType>() { CommodityType.Gold });
        controller.AddDemand(CommodityType.Diamonds);

        controller.Buy(otherParty, otherPartySupply[CommodityType.Diamonds]);

        Assert.AreEqual(1, controller.pendingToBuy.Count());

        controller.ClearPendingTransactions(CommodityType.Diamonds);

        Assert.IsEmpty(controller.pendingToBuy);
    }


    [Test]
    public void ClearPendingTransactionsTest()
    {
        Dictionary<CommodityType, Commodity> otherPartySupply =
            new Dictionary<CommodityType, Commodity>() { { CommodityType.Diamonds, new Commodity(CommodityType.Diamonds, 5, otherPartyLocation) } };
        otherParty.Supplies.Returns(otherPartySupply);
        controller.AddSupply(new Commodity(CommodityType.Gold, 5, controllerLocation));
        otherParty.Demands.Returns(new HashSet<CommodityType>() { CommodityType.Gold });
        controller.AddDemand(CommodityType.Diamonds);

        controller.Buy(otherParty, otherPartySupply[CommodityType.Diamonds]);

        Assert.AreEqual(1, controller.pendingToBuy.Count());

        controller.ClearPendingTransactions();

        Assert.IsEmpty(controller.pendingToBuy);
    }



    
}

