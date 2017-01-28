using NUnit.Framework;
using UnityEngine;
using TradingMiniGame;
using System.Linq;
using System;

[TestFixture]
public class GameGridTests
{
    private GameGrid grid;

    private class GridObjectMock : MonoBehaviour, IGridObject
    {
        private Vector3 _position;

        public GridObjectType gridObjectType
        {
            get
            {
                return GridObjectType.RegularHexagon;
            }
            set
            {
                
            }
        }

        public Vector3 size
        {
            get
            {
                return new Vector3(1, 1, 1);
            }
        }

        public void Destroy()
        {
            
        }

        public void MoveToPosition(Vector3 position)
        {
            _position = position;
        }
    }

    [TestFixtureSetUp]
    public void Setup()
    {
        grid = new GameObject().AddComponent<GameGrid>();
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
            var position = ((MonoBehaviour)gridObject).gameObject.transform.position;
            var gridIndex = grid.indexOf
        }

        Assert.AreEqual(36, grid.Count());
    }
}