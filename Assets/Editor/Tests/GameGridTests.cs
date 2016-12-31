using NUnit.Framework;
using NSubstitute.Core;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using System;

[TestFixture]
class GameGridTests
{
    private GameGrid grid;
    private GameGrid mockGrid;

    [TestFixtureSetUp]
    public void Init()
    {
        GameObject go = new GameObject();
        go.AddComponent<GameGrid>();
        grid = go.GetComponent<GameGrid>();
        grid.numberOfRows = 8;
        grid.numberOfColumns = 8;
        grid.gridObjectPrefab = ((GameObject)Resources.Load("Cube"));

        GameObject mockPrefab = GameObject.Instantiate(grid.gridObjectPrefab);
        GameObject.DestroyImmediate((UnityEngine.Object)mockPrefab.GetComponent<IGridObject>());
        mockPrefab.AddComponent<TestGridObject>();
        grid.gridObjectPrefab = mockPrefab;

        grid.Initialize();
    }
    

    static object[] GetNeighborCases =
    {
        new object[] { new GridIndex(0,0), GridDirection.left, 1, null },
        new object[] { new GridIndex(0,0), GridDirection.up, 1, null },
        new object[] { new GridIndex(0,0), GridDirection.left, 2, null },
        new object[] { new GridIndex(0,0), GridDirection.up, 2, null },
        new object[] { new GridIndex(1,0), GridDirection.left, 1, null },
        new object[] { new GridIndex(1,0), GridDirection.up, 1, new GridIndex(0,0) },
        new object[] { new GridIndex(1,0), GridDirection.left, 2, null },
        new object[] { new GridIndex(1,0), GridDirection.up, 2, null },
        new object[] { new GridIndex(0,1), GridDirection.left, 1, new GridIndex(0,0) },
        new object[] { new GridIndex(0,1), GridDirection.up, 1, null},
        new object[] { new GridIndex(0,1), GridDirection.left, 2, null },
        new object[] { new GridIndex(0,1), GridDirection.up, 2, null },
        new object[] { new GridIndex(7,7), GridDirection.down, 1, null },
        new object[] { new GridIndex(7,7), GridDirection.right, 1, null},
        new object[] { new GridIndex(7,7), GridDirection.down, 2, null },
        new object[] { new GridIndex(7,7), GridDirection.right, 2, null },
        new object[] { new GridIndex(7,6), GridDirection.down, 1, null },
        new object[] { new GridIndex(7,6), GridDirection.right, 1, new GridIndex(7,7) },
        new object[] { new GridIndex(7,6), GridDirection.down, 2, null },
        new object[] { new GridIndex(7,6), GridDirection.right, 2, null },
        new object[] { new GridIndex(6,7), GridDirection.down, 1, new GridIndex(7,7) },
        new object[] { new GridIndex(6,7), GridDirection.right, 1, null},
        new object[] { new GridIndex(6,7), GridDirection.down, 2, null },
        new object[] { new GridIndex(6,7), GridDirection.right, 2, null }
    };
    [TestCaseSource("GetNeighborCases")]
    public void GetNeighborTest(GridIndex gridObjectIndex, GridDirection direction, int index, GridIndex expectedNeighborIndex)
    {
        IGridObject gridObject = grid[gridObjectIndex];
        IGridObject result = grid.GetNeighbor(gridObject, (GridDirection)direction, index);
        GridIndex actualNeighborIndex = grid.IndexOf(result);
        Assert.AreEqual(expectedNeighborIndex, actualNeighborIndex);
    }

    static object[] GetNeighborsCases =
    {
        new object[] { new GridIndex(0,0),null,new List<GridIndex> { new GridIndex(0,1), new GridIndex(1,0) }, false },
        new object[] { new GridIndex(0,1),null,new List<GridIndex> { new GridIndex(0,0), new GridIndex(0,2), new GridIndex(1,1) }, false },
        new object[] { new GridIndex(1,0),null,new List<GridIndex> { new GridIndex(0,0), new GridIndex(1,1), new GridIndex(2,0) }, false },
        new object[] { new GridIndex(1,1),null,new List<GridIndex> { new GridIndex(0,1), new GridIndex(1,0), new GridIndex(2,1), new GridIndex(1,2) }, false },
        new object[] { new GridIndex(0,0), new List<GridIndex> { new GridIndex(1,0) },new List<GridIndex> { new GridIndex(0,1) }, true },
        new object[] { new GridIndex(0,1), new List<GridIndex> { new GridIndex(1, 1) },new List<GridIndex> { new GridIndex(0,0), new GridIndex(0,2) }, true },
        new object[] { new GridIndex(1,0), new List<GridIndex> { new GridIndex(2, 0) },new List<GridIndex> { new GridIndex(0,0), new GridIndex(1,1) }, true },
        new object[] { new GridIndex(1,1), new List<GridIndex> { new GridIndex(1, 2) },new List<GridIndex> { new GridIndex(0,1), new GridIndex(1,0), new GridIndex(2,1) }, true },
        new object[] { new GridIndex(7,7),null,new List<GridIndex> { new GridIndex(6,7), new GridIndex(7,6) }, false },
        new object[] { new GridIndex(7,6),null,new List<GridIndex> { new GridIndex(7,7), new GridIndex(7,5), new GridIndex(6,6) }, false },
        new object[] { new GridIndex(6,7),null,new List<GridIndex> { new GridIndex(7,7), new GridIndex(5,7), new GridIndex(6,6) }, false },
        new object[] { new GridIndex(6,6),null,new List<GridIndex> { new GridIndex(6,7), new GridIndex(7,6), new GridIndex(5,6), new GridIndex(6,5) }, false },
        new object[] { new GridIndex(7,7), new List<GridIndex> { new GridIndex(7,6) },new List<GridIndex> { new GridIndex(6,7) }, true },
        new object[] { new GridIndex(7,6), new List<GridIndex> { new GridIndex(6, 6) },new List<GridIndex> { new GridIndex(7,7), new GridIndex(7,5) }, true },
        new object[] { new GridIndex(6,7), new List<GridIndex> { new GridIndex(6, 6) },new List<GridIndex> { new GridIndex(7,7), new GridIndex(5,7) }, true },
        new object[] { new GridIndex(6,6), new List<GridIndex> { new GridIndex(6, 5) },new List<GridIndex> { new GridIndex(6,7), new GridIndex(7,6), new GridIndex(5,6) }, true }
    };
    [TestCaseSource("GetNeighborsCases")]
    public void GetNeighborsTest(GridIndex testGridIndex, List<GridIndex> testNeighborIndices, List<GridIndex> expectedNeighborIndices, bool colorDiff )
    {
        grid.SpawnGridObjects();
        foreach (IGridObject gridObject in grid)
        {
            gridObject.material = CubeMaterials.RedCube;
        }

        if (colorDiff)
        {
            foreach (GridIndex index in testNeighborIndices)
            {
                grid[index].material = CubeMaterials.BlueCube;
            }
        }

        List<GridIndex> actualNeighborIndices = (from p in grid.GetMatchingNeighbors(grid[testGridIndex]) select grid.IndexOf(p)).ToList();

        Assert.AreEqual(expectedNeighborIndices.Count, actualNeighborIndices.Count);
        actualNeighborIndices.ForEach(p => Assert.Contains(p, expectedNeighborIndices));
    }


    [Test]
    public void RandomizeGridExceptionTest()
    {
        Assert.DoesNotThrow(new TestDelegate(grid.RandomizeGridObjects));
    }


    private void SetUpMatchingGroups(List<List<GridIndex>> testObjects)
    {
        grid.Initialize();
        int alternate = 0;
        foreach (IGridObject gridObject in grid)
        {
            if (alternate == 0)
            {
                gridObject.material = CubeMaterials.GreenCube;
                alternate++;
            }
            else if (alternate == 1)
            {
                gridObject.material = CubeMaterials.PurpleCube;
                alternate++;
            }
            else if (alternate == 2)
            {
                gridObject.material = CubeMaterials.WhiteCube;
                alternate = 0;
            }
        }

        foreach (List<GridIndex> indexList in testObjects)
        {
            foreach (GridIndex index in indexList)
            {
                grid[index].material = CubeMaterials.BlueCube;
            }
        }
    }


    static object[] GetMatchesCases =
    {
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(0, 0), new GridIndex(0, 1), new GridIndex(0, 2) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(0, 0), new GridIndex(1, 0), new GridIndex(2, 0) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(1, 2), new GridIndex(1, 3) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(2, 1), new GridIndex(3, 1) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(7, 6), new GridIndex(7, 5) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(6, 7), new GridIndex(5, 7) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(6, 6), new GridIndex(6, 5), new GridIndex(6, 4) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(6, 6), new GridIndex(5, 6), new GridIndex(4, 6) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(1, 2), new GridIndex(1, 3), new GridIndex(1, 4) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(2, 1), new GridIndex(3, 1), new GridIndex(4, 1) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(7, 6), new GridIndex(7, 5), new GridIndex(7, 4) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(6, 7), new GridIndex(5, 7), new GridIndex(4, 7) } }, 1 },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(1,1),new GridIndex(1,2),new GridIndex(1,3) },
                                                  new List<GridIndex> { new GridIndex(6,6),new GridIndex(6,5),new GridIndex(6,4) } } , 2 },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(1,1),new GridIndex(1,2),new GridIndex(1,3) },
                                                  new List<GridIndex> { new GridIndex(6,6),new GridIndex(6,5),new GridIndex(6,4) },
                                                  new List<GridIndex> { new GridIndex(3,3),new GridIndex(3,4),new GridIndex(3,5) } } , 3 },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(1,4),new GridIndex(2,4),new GridIndex(3,4) ,
                                                                        new GridIndex(3,3),new GridIndex(3,2),new GridIndex(3,1) } }, 1 }
    };
    [Test,TestCaseSource("GetMatchesCases")]
    public void GetMatchesTest(List<List<GridIndex>> testObjects, int numberOfGroupsExpected)
    {
        SetUpMatchingGroups(testObjects);

        var actual = grid.GetMatchingGroups();
        
        bool foundEquivalentLists = true;
        foreach (List<IGridObject> gridObjectList in actual)
        {
            foundEquivalentLists = true;
            bool foundEquivalentList = true;
            foreach (List<GridIndex> gridIndexList in testObjects)
            {
                foundEquivalentList = true;
                foreach (IGridObject gridObject in gridObjectList)
                {
                    foundEquivalentList = foundEquivalentList && (gridIndexList.Contains(grid.IndexOf(gridObject)));
                }
                if(foundEquivalentList)
                {
                    break;
                }
            }
            foundEquivalentLists = foundEquivalentLists && foundEquivalentList;
        }

        Assert.True(foundEquivalentLists);
        Assert.AreEqual(numberOfGroupsExpected, actual.Count);
    }



    static object[] ResolveMatchesTestCases =
    {
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(0, 0), new GridIndex(0, 1), new GridIndex(0, 2) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(0, 0), new GridIndex(1, 0), new GridIndex(2, 0) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(1, 2), new GridIndex(1, 3) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(2, 1), new GridIndex(3, 1) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(7, 6), new GridIndex(7, 5) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(6, 7), new GridIndex(5, 7) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(6, 6), new GridIndex(6, 5), new GridIndex(6, 4) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(6, 6), new GridIndex(5, 6), new GridIndex(4, 6) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(1, 2), new GridIndex(1, 3), new GridIndex(1, 4) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(2, 1), new GridIndex(3, 1), new GridIndex(4, 1) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(7, 6), new GridIndex(7, 5), new GridIndex(7, 4) } }, 1 },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(6, 7), new GridIndex(5, 7), new GridIndex(4, 7) } }, 1 },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(1,1),new GridIndex(1,2),new GridIndex(1,3) },
                                                  new List<GridIndex> { new GridIndex(6,6),new GridIndex(6,5),new GridIndex(6,4) } } , 2 },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(1,1),new GridIndex(1,2),new GridIndex(1,3) },
                                                  new List<GridIndex> { new GridIndex(6,6),new GridIndex(6,5),new GridIndex(6,4) },
                                                  new List<GridIndex> { new GridIndex(3,3),new GridIndex(3,4),new GridIndex(3,5) } } , 3 },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(1,4),new GridIndex(2,4),new GridIndex(3,4) ,
                                                                        new GridIndex(3,3),new GridIndex(3,2),new GridIndex(3,1) } }, 1 }
    };
    [TestCaseSource("ResolveMatchesTestCases")]
    public void ResolveMatchesTest(List<List<GridIndex>> testObjects, int numberOfGroupsExpected)
    {
        TestGridObject.DestroyCalledOn.Clear();

        SetUpMatchingGroups(testObjects);

        List<IGridObject> expectedToBeDestroyed = new List<IGridObject>();
        testObjects.ForEach(p => expectedToBeDestroyed.AddRange((from q in p select grid[q])));

        List<IGridObject> actualToBeDestroyed = new List<IGridObject>();
        grid.GetMatchingGroups().ForEach(p => actualToBeDestroyed.AddRange((from q in p select q)));

        grid.ResolveMatches();

        Assert.AreEqual(64, grid.Count());
        Assert.AreEqual(expectedToBeDestroyed.Count, actualToBeDestroyed.Count);
        expectedToBeDestroyed.ForEach(p => Assert.Contains(p, actualToBeDestroyed));
        expectedToBeDestroyed.ForEach(p => Assert.False(grid.Contains(p)));
        grid.ToList().ForEach(p => Assert.NotNull(p));
    }


    static object[] ResolveMatchesIndexShiftTestCases =
    {
        new object[] { new List<GridIndex> { new GridIndex(1, 0), new GridIndex(1, 1), new GridIndex(1, 2) },
                       new List<GridIndex> { new GridIndex(0, 0), new GridIndex(0, 1), new GridIndex(0, 2) },
                       new List<GridIndex> { new GridIndex(1, 0), new GridIndex(1, 1), new GridIndex(1, 2) } },
        new object[] { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(6, 7), new GridIndex(5, 7) },
                       new List<GridIndex> { new GridIndex(2, 7), new GridIndex(3, 7), new GridIndex(4, 7) },
                       new List<GridIndex> { new GridIndex(5, 7), new GridIndex(6, 7), new GridIndex(7, 7) } },
        new object[] { new List<GridIndex> { new GridIndex(3, 1), new GridIndex(3, 2), new GridIndex(3, 3), new GridIndex(2, 2) },
                       new List<GridIndex> { new GridIndex(2, 1), new GridIndex(1, 2), new GridIndex(2, 3), new GridIndex(0, 2) },
                       new List<GridIndex> { new GridIndex(3, 1), new GridIndex(3, 2), new GridIndex(3, 3), new GridIndex(2, 2) } }
    };
    [TestCaseSource("ResolveMatchesIndexShiftTestCases")]
    public void ResolveMatchesIndexShiftTest(List<GridIndex> testIndices, List<GridIndex> indicesToShift, List<GridIndex> expectedIndices)
    {
        SetUpMatchingGroups(new List<List<GridIndex>> { testIndices });

        List<IGridObject> shiftedObjects = (from p in indicesToShift select grid[p]).ToList();

        grid.ResolveMatches();

        List<GridIndex> actualIndices = (from p in shiftedObjects select grid.IndexOf(p)).ToList();

        for(int i =0; i < expectedIndices.Count; i++)
        {
            Assert.AreEqual(expectedIndices[i], actualIndices[i]);
        }
    }


    [Test]
    public void SpawnGridObjectsTest()
    {
        Assert.AreEqual(64, grid.Count());
        foreach(IGridObject gridObject in grid)
        {
            Assert.NotNull(gridObject);
        }
    }

    //[Test]
    //public void SwapCubesTest()
    //{
    //    SetUpMatchingGroups()
    //}
}

