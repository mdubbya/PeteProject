using NUnit.Framework;

[TestFixture]
public class CubeTests
{
    [Test]
    public void GetNeighborTest()
    {
        Cube cube = new Cube();
        GridIndex extents = new GridIndex(8, 8);
        cube.gridPosition = new GridIndex(3, 3);
        GridIndex result = cube.GetNeighbor(GridDirection.left, extents);

        Assert.AreEqual(2, result.x);
        Assert.AreEqual(3, result.y);
    }
}