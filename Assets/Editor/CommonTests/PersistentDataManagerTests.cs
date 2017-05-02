using NUnit.Framework;
using NSubstitute;
using Common;
using System;

[TestFixture]
public class PersistentDataManagerTests
{
    [Serializable]
    public class PersistentDataObjectTest : PersistentDataObject<PersistentDataObjectTest>
    {
        public int value { get; set; }
        private PersistentDataObjectTest() { }
    }
    

    [Test]
    public void ReadWriteDataTest()
    {
        PersistentDataObjectTest.Instance.value = 10;
        PersistentDataObjectTest.Instance.WriteData();
        PersistentDataObjectTest.Instance.value = 0;
        PersistentDataObjectTest.Instance.ReadData();
        
        Assert.AreEqual(10, PersistentDataObjectTest.Instance.value);
    }
}

