using NUnit.Framework;
using UnityEngine;
using MatchThreeMiniGame;

[TestFixture]
class CubeTests
{
    private Cube cube;

    [Test]
    public void MovePositionTest()
    {
        GameObject obj = new GameObject();
        cube = obj.AddComponent<Cube>();
        cube.speed = 100000;

        Vector3 originalPosition = cube.gameObject.transform.position;
        Vector3 testPosition = new Vector3(3, 3, 3);
        cube.MoveToPosition(testPosition);
        cube.Update();

        Assert.AreNotEqual(originalPosition, testPosition);
        Assert.AreEqual(testPosition, cube.gameObject.transform.position);
    }

    [Test]
    public void DestroyCubeTest()
    {
        GameObject obj = new GameObject();
        cube = obj.AddComponent<Cube>();
        
        cube.Destroy();
        if (obj == null && cube == null)
        {
            Assert.Pass();
        }
    }
}
