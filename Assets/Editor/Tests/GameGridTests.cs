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

    [TestFixtureSetUp]
    public void Init()
    {
        GameObject go = new GameObject();
        go.AddComponent<GameGrid>();
        grid = go.GetComponent<GameGrid>();
        grid.numberOfRows = 8;
        grid.numberOfColumns = 8;
        grid.gridObjectPrefab = ((GameObject)Resources.Load("Cube"));
        
        grid.Initialize();
    }
    

    private void SetUpMatchingGroups(List<List<GridIndex>> testObjects)
    {
        grid.Initialize();

        foreach (IGridObject gridObject in grid)
        {
            gridObject.gridObjectType = GridObjectType.None;
        }

        foreach (List<GridIndex> indexList in testObjects)
        {
            foreach (GridIndex index in indexList)
            {
                grid[index].gridObjectType = GridObjectType.BlueCube;
            }
        }
    }


    static object[] ResolveMatchesTestCases =
    {
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(0, 0), new GridIndex(0, 1), new GridIndex(0, 2) } }, new List<bool> { true } },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(0, 0), new GridIndex(1, 0), new GridIndex(2, 0) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(1, 2), new GridIndex(1, 3) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(2, 1), new GridIndex(3, 1) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(7, 6), new GridIndex(7, 5) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(6, 7), new GridIndex(5, 7) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(6, 6), new GridIndex(6, 5), new GridIndex(6, 4) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(6, 6), new GridIndex(5, 6), new GridIndex(4, 6) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(1, 2), new GridIndex(1, 3), new GridIndex(1, 4) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(1, 1), new GridIndex(2, 1), new GridIndex(3, 1), new GridIndex(4, 1) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(7, 6), new GridIndex(7, 5), new GridIndex(7, 4) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(6, 7), new GridIndex(5, 7), new GridIndex(4, 7) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(1,1),new GridIndex(1,2),new GridIndex(1,3) },
                                                  new List<GridIndex> { new GridIndex(6,6),new GridIndex(6,5),new GridIndex(6,4) } }, new List<bool> { true, true }  },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(1,1),new GridIndex(1,2),new GridIndex(1,3) },
                                                  new List<GridIndex> { new GridIndex(6,6),new GridIndex(6,5),new GridIndex(6,4) },
                                                  new List<GridIndex> { new GridIndex(3,3),new GridIndex(3,4),new GridIndex(3,5) } }, new List<bool> { true,true,true }  },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(1,4),new GridIndex(2,4),new GridIndex(3,4) ,
                                                                        new GridIndex(3,3),new GridIndex(3,2),new GridIndex(3,1) } }, new List<bool> { true }  },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(0,0),new GridIndex(0,1),new GridIndex(1,0) } }, new List<bool> { false } },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(0,0),new GridIndex(0,1),new GridIndex(1,1), new GridIndex(1,2) } }, new List<bool> { false } },
        new object[] { new List<List<GridIndex>> {new List<GridIndex> { new GridIndex(3,2),new GridIndex(3,3),new GridIndex(3,4) },
                                                  new List<GridIndex> { new GridIndex(4,2),new GridIndex(4,3) } }, new List<bool> { true,false } }
    };
    [TestCaseSource("ResolveMatchesTestCases")]
    public void ResolveMatchesTest(List<List<GridIndex>> testObjects, List<bool> shouldBeDestroyed)
    {
        SetUpMatchingGroups(testObjects);

        List<IGridObject> expectedToBeDestroyed = new List<IGridObject>();
        List<IGridObject> expectedNotToBeDestroyed = new List<IGridObject>();
        testObjects.ForEach(p => expectedToBeDestroyed.AddRange((from q in p.Where(n=>shouldBeDestroyed[testObjects.IndexOf(p)]) select grid[q])));
        testObjects.ForEach(p => expectedNotToBeDestroyed.AddRange((from q in p.Where(n => !shouldBeDestroyed[testObjects.IndexOf(p)]) select grid[q])));
        grid.ResolveMatches(false);

        Assert.AreEqual(64, grid.Count());
        expectedToBeDestroyed.ForEach(p => Assert.False(grid.Contains(p)));
        expectedNotToBeDestroyed.ForEach(p => Assert.True(grid.Contains(p)));
        grid.ToList().ForEach(p => Assert.NotNull(p));
    }


    object[] ResolveMatchesOnlyThreeOrMoreTestCases =
    {
        new List<GridIndex>() { new GridIndex(0,0), new GridIndex(0,1) },
        new List<GridIndex>() { new GridIndex(0,0), new GridIndex(0,1), new GridIndex(0,2) }
    };
    [TestCaseSource("ResolveMatchesOnlyThreeOrMoreTestCases")]
    public void ResolveMatchesOnlyThreeOrMoreTest(List<GridIndex> testIndices)
    {
        SetUpMatchingGroups(new List<List<GridIndex>> { testIndices });

        List<IGridObject> shouldExist = testIndices.Select(p => grid[p]).ToList();

        grid.ResolveMatches();

        if (testIndices.Count < 3)
        {
            shouldExist.ForEach(p => Assert.True(grid.Contains(p)));
        }
        else
        {
            shouldExist.ForEach(p => Assert.False(grid.Contains(p)));
        }
    }


    static object[] ResolveMatchesCascadeTestCases =
    {
        new object[] { new List<GridIndex> { new GridIndex(1, 0), new GridIndex(1, 1), new GridIndex(1, 2) } ,
                       new List<GridIndex> { new GridIndex(0, 2), new GridIndex(1, 3), new GridIndex(1, 4) } },
    };
    [TestCaseSource("ResolveMatchesCascadeTestCases")]
    public void ResolveMatchesCascadeTest(List<GridIndex> matches, List<GridIndex> cascadeMatches)
    {
        grid.Initialize();
        SetUpMatchingGroups(new List<List<GridIndex>> { matches });

        cascadeMatches.ForEach(p => grid[p].gridObjectType = GridObjectType.RedCube);

        List<IGridObject> matchingGridObjects = (from p in matches select grid[p]).ToList();
        matchingGridObjects.AddRange((from p in cascadeMatches select grid[p]).ToList());

        grid.ResolveMatches();

        Assert.AreEqual(64, grid.Count());
        matchingGridObjects.ForEach(p => Assert.False(grid.Contains(p)));
        grid.ToList().ForEach(p => Assert.NotNull(p));
    }


    static object[] ResolveMatchesIndexShiftTestCases =
    {
        new object[] { new List<GridIndex> { new GridIndex(1, 0), new GridIndex(1, 1), new GridIndex(1, 2) },
                       new List<GridIndex> { new GridIndex(0, 0), new GridIndex(0, 1), new GridIndex(0, 2) },
                       new List<GridIndex> { new GridIndex(1, 0), new GridIndex(1, 1), new GridIndex(1, 2) } },
        new object[] { new List<GridIndex> { new GridIndex(7, 7), new GridIndex(6, 7), new GridIndex(5, 7) },
                       new List<GridIndex> { new GridIndex(2, 7), new GridIndex(3, 7), new GridIndex(4, 7) },
                       new List<GridIndex> { new GridIndex(5, 7), new GridIndex(6, 7), new GridIndex(7, 7) } }
    };
    [TestCaseSource("ResolveMatchesIndexShiftTestCases")]
    public void ResolveMatchesIndexShiftTest(List<GridIndex> testIndices, List<GridIndex> indicesToShift, List<GridIndex> expectedIndices)
    {
        SetUpMatchingGroups(new List<List<GridIndex>> { testIndices });

        List<IGridObject> shiftedObjects = (from p in indicesToShift select grid[p]).ToList();


        grid.ResolveMatches(false);

        List<GridIndex> actualIndices = (from p in shiftedObjects select grid.IndexOf(p)).ToList();

        for(int i =0; i < expectedIndices.Count; i++)
        {
            Assert.AreEqual(expectedIndices[i], actualIndices[i]);
        }
    }


    static object[] SwapGridObjectsTestCases =
    {
        new object[] { new GridIndex(0,0), new GridIndex(0,1), new List<GridIndex>() { new GridIndex(0, 2), new GridIndex(0, 3) }, true  },
        new object[] { new GridIndex(7,7), new GridIndex(7,6), new List<GridIndex>() { new GridIndex(7, 5), new GridIndex(7, 4) }, true  },
        new object[] { new GridIndex(0,1), new GridIndex(1,1), new List<GridIndex>() { new GridIndex(1,0), new GridIndex(1,2)}, true },
        new object[] { new GridIndex(0,0), new GridIndex(7,7), new List<GridIndex>() { new GridIndex(7, 6), new GridIndex(7, 5) }, false },
        new object[] { new GridIndex(0,0), new GridIndex(0,1), new List<GridIndex>() { new GridIndex(0, 2) }, false  }
    };
    [TestCaseSource("SwapGridObjectsTestCases")]
    public void SwapGridObjectsTest(GridIndex firstIndex, GridIndex secondIndex, List<GridIndex> otherMatching, bool shouldSwap)
    {
        List<List<GridIndex>> objectsForSetup = new List<List<GridIndex>>() { new List<GridIndex> { firstIndex } };
        objectsForSetup.Add(otherMatching);
        SetUpMatchingGroups(objectsForSetup);

        IGridObject firstObject = grid[firstIndex];
        IGridObject secondObject = grid[secondIndex];

        grid.SwapGridObjects(firstObject, secondObject);

        if (shouldSwap)
        {
            Assert.AreEqual(firstIndex, grid.IndexOf(secondObject));
            Assert.AreEqual(secondIndex, grid.IndexOf(firstObject));
        }
        else
        {
            Assert.AreEqual(firstIndex, grid.IndexOf(firstObject));
            Assert.AreEqual(secondIndex, grid.IndexOf(secondObject));
        }
    }

}

