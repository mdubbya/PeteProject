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
            IGridObject gridObject = new GridObject();
            gridObject.pathCost = int.MaxValue;
            return gridObject;
        });
         
        
        grid = new GameGridController();
        grid.Init(gridSub, factorySub);
    }

    static object[] BuildGridTestCases =
    {
        new object[] {new GridIndex(0,0),
            new List<GridDirection>() { GridDirection.N,GridDirection.NE } },
        new object[] {new GridIndex(1,0),
            new List<GridDirection>() { GridDirection.N,GridDirection.NE,GridDirection.SE,GridDirection.S }},
        new object[] {new GridIndex(5,5),
            new List<GridDirection>() { GridDirection.S, GridDirection.SW}},
        new object[] {new GridIndex(4,5),
            new List<GridDirection>() { GridDirection.N, GridDirection.NW, GridDirection.SW, GridDirection.S } }
    };
    [TestCaseSource("BuildGridTestCases")]
    public void BuildGridTest(GridIndex testIndex, List<GridDirection> expectedNeighbors)
    {
        grid.start = new GridIndex(0, 0);
        grid.end = new GridIndex(5, 5);
        grid.BuildGrid(6, 6);
        foreach(IGridObject gridObject in grid)
        {
            Assert.AreNotEqual(null, gridObject);
        }
        Assert.AreEqual(36, grid.Count());

        foreach(GridDirection index in expectedNeighbors)
        {
            Assert.False(grid.GetAdjacentDisconnected(testIndex).Contains(index));
        }
    }


    static object[] GetShortestPathTestCases =
    {
        new object[] {
            new GridIndex(1,0), //start
            new GridIndex(3,5), //end
            new List<GridIndex>() { //gridObjectCostToAlter
                new GridIndex(1,1), new GridIndex(2,2),
                new GridIndex(2,3), new GridIndex(3,4)},
            new List<List<GridDirection>>() {
                new List<GridDirection>(),new List<GridDirection>(),new List<GridDirection>(),
                new List<GridDirection>(),new List<GridDirection>(),new List<GridDirection>() },//directionsToRemove
            new List<int>() {1,1,1,1}, //gridObjectCosts
            true,
            4 },//expected

        new object[] {
            new GridIndex(1,0), //start
            new GridIndex(3,5), //end
            new List<GridIndex>() { //gridObjectCostToAlter
                new GridIndex(1,1), new GridIndex(2,2),
                new GridIndex(2,3), new GridIndex(3,4)},
            new List<List<GridDirection>>() { //directionsToRemove
                new List<GridDirection>() ,
                new List<GridDirection>() ,
                new List<GridDirection>() { GridDirection.N,GridDirection.NW,GridDirection.S,GridDirection.SE,GridDirection.SW, GridDirection.NE },
                new List<GridDirection>() },
            new List<int>() {1,1,1,1,1,1 }, //gridObjectCosts
            true,
            23 },//expected

        new object[] {
            new GridIndex(1,0), //start
            new GridIndex(3,5), //end
            new List<GridIndex>() { }, //gridObjectCostToAlter
            new List<List<GridDirection>>(), //directionsToRemove
            new List<int>() { }, //gridObjectCosts
            true,
            40 }, //expected

        new object[] {
            new GridIndex(1,0), //start
            new GridIndex(3,5), //end
            new List<GridIndex>() {
                new GridIndex(1,5),new GridIndex(2,4),new GridIndex(2,3),
                new GridIndex(3,2),new GridIndex(3,1),new GridIndex(4,0), }, //gridObjectCostToAlter
            new List<List<GridDirection>>() { //directionsToRemove
                new List<GridDirection>() { GridDirection.N,GridDirection.NE,GridDirection.NW,GridDirection.S,GridDirection.SE,GridDirection.SW },
                new List<GridDirection>() { GridDirection.N,GridDirection.NE,GridDirection.NW,GridDirection.S,GridDirection.SE,GridDirection.SW },
                new List<GridDirection>() { GridDirection.N,GridDirection.NE,GridDirection.NW,GridDirection.S,GridDirection.SE,GridDirection.SW },
                new List<GridDirection>() { GridDirection.N,GridDirection.NE,GridDirection.NW,GridDirection.S,GridDirection.SE,GridDirection.SW },
                new List<GridDirection>() { GridDirection.N,GridDirection.NE,GridDirection.NW,GridDirection.S,GridDirection.SE,GridDirection.SW },
                new List<GridDirection>() { GridDirection.N,GridDirection.NE,GridDirection.NW,GridDirection.S,GridDirection.SE,GridDirection.SW } },
            new List<int>() {1,1,1,1,1,1 }, //gridObjectCosts
            false,
            0 }, //expected

        new object[]
        {
            new GridIndex(1,0),
            new GridIndex(4,4),
            new List<GridIndex>(){
                new GridIndex(1,1), new GridIndex(2,2), new GridIndex(2,3), new GridIndex(3,4),
                new GridIndex(2,1), new GridIndex(3,1), new GridIndex(4, 2), new GridIndex(4,3) },
            new List<List<GridDirection>>(){
                new List<GridDirection> () { GridDirection.NE },
                new List<GridDirection>() { },
                new List<GridDirection>() { },
                new List<GridDirection>() { },
                new List<GridDirection>() { },
                new List<GridDirection>() { },
                new List<GridDirection>() { },
                new List<GridDirection>() { }},
            new List<int>() {2,1,1,1,2,2,2,2 },
            true,
            7 }
        };

    
    [TestCaseSource("GetShortestPathTestCases")] 
    public void GetShortestPathTest(GridIndex start, GridIndex end, 
        List<GridIndex> gridObjectCostToAlter, List<List<GridDirection>> directionsToRemove, List<int> gridObjectCosts, bool expectedPossible, float expected)
    {
        grid.start = start;
        grid.end = end;
        grid.BuildGrid(6, 6);

        foreach(IGridObject obj in grid)
        {
            obj.pathCost = 10;
        }
        grid[end].pathCost = 0;
        grid[start].pathCost = 0;

        for(int i=0; i < gridObjectCostToAlter.Count; i++)
        {
            grid[gridObjectCostToAlter[i]].pathCost = gridObjectCosts[i];
            directionsToRemove[i].ForEach(p => grid.RemoveNeighbor(gridObjectCostToAlter[i],p));
        }

        List<GridIndex> path = grid.GetShortestPath();
        if (expectedPossible)
        {
            float len = path.Sum(p => grid[p].pathCost);
            Assert.AreEqual(expected, len);
        }
        else
        {
            Assert.IsNull(path);
        }
    }


    static object[] SelectIndexTestCases =
    {
        new object[] { new List<GridIndex>() { new GridIndex(0, 0) }, true },
        new object[] { new List<GridIndex>() { new GridIndex(0, 0), new GridIndex(0, 1),new GridIndex(0, 2),
            new GridIndex(0, 3), new GridIndex(0, 4), new GridIndex(0, 5), new GridIndex(1, 5), new GridIndex(2, 5),
            new GridIndex(3, 5)}, true },
        new object[] { new List<GridIndex>() { new GridIndex(1, 2) }, false },
        new object[] { new List<GridIndex>() { new GridIndex(3, 5) }, false },
        new object[] { new List<GridIndex>() { new GridIndex(1, 1), new GridIndex(2, 2) }, false }
    };

    [TestCaseSource("SelectIndexTestCases")]
    public void SelectIndexTest(List<GridIndex> testPath, bool expected)
    {
        grid.start = new GridIndex(1, 0);
        grid.end = new GridIndex(3, 5);
        grid.BuildGrid(6, 6);

        foreach (IGridObject obj in grid)
        {
            obj.pathCost = 10;
        }
        
        grid.RemoveNeighbor(new GridIndex(1, 1), GridDirection.NE);
        grid.RemoveNeighbor(new GridIndex(2, 1), GridDirection.NE);
        grid.RemoveNeighbor(new GridIndex(2, 1), GridDirection.SE);
        grid.RemoveNeighbor(new GridIndex(3, 1), GridDirection.SE);
        grid.RemoveNeighbor(new GridIndex(4, 2), GridDirection.S);
        grid.RemoveNeighbor(new GridIndex(4, 2), GridDirection.SE);
        grid.RemoveNeighbor(new GridIndex(4, 3), GridDirection.S);
        grid.RemoveNeighbor(new GridIndex(4, 4), GridDirection.SW);
        grid.RemoveNeighbor(new GridIndex(3, 4), GridDirection.NW);
        grid.RemoveNeighbor(new GridIndex(3, 4), GridDirection.SW);
        grid.RemoveNeighbor(new GridIndex(2, 4), GridDirection.NW);
        grid.RemoveNeighbor(new GridIndex(1, 3), GridDirection.N);
        grid.RemoveNeighbor(new GridIndex(1, 3), GridDirection.NW);
        grid.RemoveNeighbor(new GridIndex(1, 2), GridDirection.N);

        bool lastResult=false;
        foreach(GridIndex index in testPath)
        {
            lastResult = grid.SelectIndex(index);
        }

        Assert.AreEqual(expected, lastResult);
    }


    static object[] GetSelectedPathTestCases =
    {
        new object[] 
        {
            new List<GridIndex>() { new GridIndex(0, 0) },
            new List<GridIndex>() { new GridIndex(1, 0), new GridIndex(0, 0) }
        },
        new object[] 
        {
            new List<GridIndex>() { new GridIndex(0, 0), new GridIndex(0, 1),new GridIndex(0, 2),
                new GridIndex(0, 3), new GridIndex(0, 4), new GridIndex(0, 5), new GridIndex(1, 5), new GridIndex(2, 5),
                new GridIndex(3, 5)},
            new List<GridIndex>() {new GridIndex(1,0), new GridIndex(0, 0), new GridIndex(0, 1),new GridIndex(0, 2),
                new GridIndex(0, 3), new GridIndex(0, 4), new GridIndex(0, 5), new GridIndex(1, 5), new GridIndex(2, 5),
                new GridIndex(3, 5)}
        },
        new object[] 
        {
            new List<GridIndex>() { new GridIndex(1, 2) },
            new List<GridIndex>() {new GridIndex(1,0) }
        },
        new object[] 
        {
            new List<GridIndex>() { new GridIndex(3, 5) },
            new List<GridIndex>() {new GridIndex(1,0) }
        },
        new object[] 
        {
            new List<GridIndex>() { new GridIndex(1, 1), new GridIndex(2, 2) },
            new List<GridIndex>() {new GridIndex(1,0), new GridIndex(1, 1) }
        },
        new object[]
        {
            new List<GridIndex>() { new GridIndex(0, 0), new GridIndex(0, 1),new GridIndex(0, 2),
                new GridIndex(0, 3), new GridIndex(0, 4), new GridIndex(0, 5), new GridIndex(1, 5), new GridIndex(2, 5),
                new GridIndex(3, 5), new GridIndex(0,2)},
            new List<GridIndex>() {new GridIndex(1,0), new GridIndex(0, 0), new GridIndex(0, 1),new GridIndex(0, 2)}
        }
    };
    [TestCaseSource("GetSelectedPathTestCases")]
    public void GetSelectedPathTest(List<GridIndex> testPath, List<GridIndex> expectedPath)
    {
        grid.start = new GridIndex(1, 0);
        grid.end = new GridIndex(3, 5);
        grid.BuildGrid(6, 6);

        foreach (IGridObject obj in grid)
        {
            obj.pathCost = 10;
        }

        grid.RemoveNeighbor(new GridIndex(1, 1), GridDirection.NE);
        grid.RemoveNeighbor(new GridIndex(2, 1), GridDirection.NE);
        grid.RemoveNeighbor(new GridIndex(2, 1), GridDirection.SE);
        grid.RemoveNeighbor(new GridIndex(3, 1), GridDirection.SE);
        grid.RemoveNeighbor(new GridIndex(4, 2), GridDirection.S);
        grid.RemoveNeighbor(new GridIndex(4, 2), GridDirection.SE);
        grid.RemoveNeighbor(new GridIndex(4, 3), GridDirection.S);
        grid.RemoveNeighbor(new GridIndex(4, 4), GridDirection.SW);
        grid.RemoveNeighbor(new GridIndex(3, 4), GridDirection.NW);
        grid.RemoveNeighbor(new GridIndex(3, 4), GridDirection.SW);
        grid.RemoveNeighbor(new GridIndex(2, 4), GridDirection.NW);
        grid.RemoveNeighbor(new GridIndex(1, 3), GridDirection.N);
        grid.RemoveNeighbor(new GridIndex(1, 3), GridDirection.NW);
        grid.RemoveNeighbor(new GridIndex(1, 2), GridDirection.N);

        testPath.ForEach(p => grid.SelectIndex(p));
        List<GridIndex> actualPath = grid.GetSelectedPath();
        
        Assert.AreEqual(expectedPath.Count, actualPath.Count);

        for(int i=0; i < actualPath.Count; i++)
        {
            Assert.AreEqual(expectedPath[i], actualPath[i]);
        }
    }
}