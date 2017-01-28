using NUnit.Framework;
using UnityEngine;
using TradingMiniGame;
using System.Linq;

[TestFixture]
public class GameGridTests
{
    private GameGrid grid;

    [TestFixtureSetUp]
    public void Setup()
    {
        grid = new GameObject().AddComponent<GameGrid>();
        grid.rows = 6;
        grid.columns = 6;
    }

    [Test]
    public void PopulateTest()
    {
        grid.Populate();

        foreach(IGridObject gridObject in grid)
        {
            Assert.NotNull(gridObject);
        }

        Assert.AreEqual(36, grid.Count());
    }
}