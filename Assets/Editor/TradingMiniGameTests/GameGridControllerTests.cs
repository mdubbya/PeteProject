using NUnit.Framework;
using UnityEngine;
using TradingMiniGame;
using System.Linq;
using NSubstitute;
using Zenject;
using System.Collections.Generic;

[TestFixture]
public class GameGridTests
{
    private GameGridController grid;

    [TestFixtureSetUp]
    public void Setup()
    {
        IFactory<IGridObject> factorySub = Substitute.For<IFactory<IGridObject>>();
        IGameGrid gridSub = Substitute.For<IGameGrid>();

        factorySub.Create().ReturnsForAnyArgs(x => {
            IGridObject gridObject = new HexGridObject();
            gridObject.pathCost = float.MaxValue;
            return gridObject;
        });
         
        
        grid = new GameGridController(gridSub,factorySub);
        grid.BuildGrid(6, 6);
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
    public void ReplaceGridObjectTest()
    {
             
    }


    static object[] GetPathLengthTestCases =
    {
        new object[] { new GridIndex(1,0), new GridIndex(3,5),
            new List<GridIndex>() { new GridIndex(1,0), new GridIndex(1,1), new GridIndex(2,2),new GridIndex(2,3), new GridIndex(3,4),new GridIndex(3,5 )},
            new List<float>() {1,1,1,1,1,1 },
            5 },
        new object[] { new GridIndex(1,0), new GridIndex(3,5),
            new List<GridIndex>() { },
            new List<float>() { },
            0 }
    };

    
    [TestCaseSource("GetPathLengthTestCases")] 
    public void GetPathLengthTest(GridIndex start, GridIndex end, 
        List<GridIndex> gridObjectCostToAlter, List<float> gridObjectCosts, float expected)
    {
        grid.start = grid[start];
        grid.end = grid[end];

        foreach(IGridObject obj in grid)
        {
            obj.pathCost = float.MaxValue;
        }

        for(int i=0; i < gridObjectCostToAlter.Count; i++)
        {
            grid[gridObjectCostToAlter[i]].pathCost = gridObjectCosts[i];
        }        

        float len = grid.GetShortestPath().Sum(p => grid[p].pathCost);

        Assert.AreEqual(expected, len);
    }


    [Test]
    public void GetPathValidTest()
    {

    }


}