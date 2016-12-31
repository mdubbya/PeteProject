using NUnit.Framework;

[TestFixture]
public class GameGridTests
{
    [Test]
    public void GetNeighborTest()
    {
        GameGrid grid = new GameGrid();
        grid.rows = 8;
        grid.columns = 8;

        Cube cube = new Cube();
        cube.gridPosition = new GridIndex(3, 3);

        

    }
}