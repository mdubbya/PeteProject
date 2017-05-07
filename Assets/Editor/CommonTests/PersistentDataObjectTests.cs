using NUnit.Framework;
using NSubstitute;
using Common;
using System;

[TestFixture]
public class PersistentDataObjectTests
{
    [Serializable]
    public class PersistentDataObjectTestClass : PersistentDataObject<PersistentDataObjectTestClass>
    {
        public int value { get; set; }
        private PersistentDataObjectTestClass() { }
    }
    

    [Test]
    public void ReadWriteDataTest()
    {
        PersistentDataObjectTestClass.Instance.value = 10;
        PersistentDataObjectTestClass.WriteData();
        PersistentDataObjectTestClass.Instance.value = 0;
        PersistentDataObjectTestClass.ReadData();
        
        Assert.AreEqual(10, PersistentDataObjectTestClass.Instance.value);
    }
}

