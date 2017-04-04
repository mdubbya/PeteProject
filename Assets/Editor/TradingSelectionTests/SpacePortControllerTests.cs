using NSubstitute;
using NUnit.Framework;
using TradingSelection;

[TestFixture]
public class SpacePortControllerTests
{
    ISpacePortController otherPort;
    SpacePortController controller;

    [SetUp]
    public void Setup()
    {
        otherPort = Substitute.For<ISpacePortController>();
        controller = new SpacePortController();
    }


    [Test]
    public void EstablishTradeLaneTest()
    {
        controller.EstablishTradeLane(otherPort);
        Assert.IsTrue(controller.TradeLaneEstablished(otherPort));
    }

    
    [Test]
    public void CancelTradeLaneTest()
    {
        controller.EstablishTradeLane(otherPort);
        controller.CancelTradeLane(otherPort);
        Assert.IsFalse(controller.TradeLaneEstablished(otherPort));
    }
}

