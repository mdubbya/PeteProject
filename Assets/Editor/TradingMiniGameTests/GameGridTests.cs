using NUnit.Framework;
using UnityEngine;
using TradingMiniGame;
using System.Linq;
using System;
using TradingMiniGame.Mocks;

[TestFixture]
public class GameGridTests
{
    private GameGrid grid;

    [TestFixtureSetUp]
    public void Setup()
    {
        grid = new GameGrid();
        grid.rows = 6;
        grid.columns = 6;

        GameObject gridObjectPrefab = new GameObject();
        gridObjectPrefab.AddComponent<GridObjectMock>();
        grid.gridObjectPrefab = gridObjectPrefab;
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