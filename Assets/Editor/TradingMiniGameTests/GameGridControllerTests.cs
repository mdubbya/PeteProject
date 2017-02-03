using NUnit.Framework;
using UnityEngine;
using TradingMiniGame;
using System.Linq;
using NSubstitute;

[TestFixture]
public class GameGridTests
{
    private GameGridController grid;

    [TestFixtureSetUp]
    public void Setup()
    {
        IGameGrid gridSub = Substitute.For<IGameGrid>();


        for(int row= 0; row < 6; row++)
        {
            for(int column =0; column < 6; column++)
            {
                IGridObject gridObjectSub = Substitute.For<IGridObject>();
                gridObjectSub.pathCost.Returns(1);
                gridObjectSub.gridObjectType.Returns(GridObjectType.HexagonNormal);
                gridSub.SpawnGridObject(row, column).Returns(gridObjectSub);
            }
        }

        var bob = gridSub.SpawnGridObject(0, 0);

        grid = new GameGridController(gridSub);
    }
    

    [Test]
    public void BuildGridTest()
    {
        grid.BuildGrid(6, 6);
        foreach(IGridObject gridObject in grid)
        {
            Assert.AreNotEqual(null, gridObject);
        }
        Assert.AreEqual(36, grid.Count());
    }


    [Test]
    public void ReplaceObjectTest()
    {
         
    }


    [Test] 
    public void GetPathLengthTest()
    {

    }
}