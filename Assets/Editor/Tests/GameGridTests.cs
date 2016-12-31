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
        SetUpMatchingGroups(testObjects);

        List<IGridObject> expectedToBeDestroyed = new List<IGridObject>();
        testObjects.ForEach(p => expectedToBeDestroyed.AddRange((from q in p select grid[q])));

        grid.ResolveMatches();

        Assert.AreEqual(64, grid.Count());
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
}

