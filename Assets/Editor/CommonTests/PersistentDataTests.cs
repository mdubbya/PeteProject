using NUnit.Framework;
using NSubstitute;
using Common;
using System;

[TestFixture]
public class PersistentDataTests
{
    [Serializable]
    public class PersistentDataObjectTestClass : PersistentDataObject<PersistentDataObjectTestClass>
    {
        public int value { get; set; }
        private PersistentDataObjectTestClass() { }
    }

    [Serializable]
    public class PersistentDataObjectTestClass2 : PersistentDataObject<PersistentDataObjectTestClass2>
    {
        public int value { get; set; }
        private PersistentDataObjectTestClass2() { }
    }

    [Serializable]
    public class PersistentDataObjectTestClass3 : PersistentDataObject<PersistentDataObjectTestClass3>
    {
        public int value { get; set; }
    }

    [Test]
    public void ReadWriteDataTest()
    {
        PersistentDataObjectTestClass.Instance.value = 10;
        PersistentDataObjectTestClass2.Instance.value = 50;
        PersistentDataManager.WriteData("test");

        PersistentDataObjectTestClass.Instance.value = 0;
        PersistentDataObjectTestClass.Instance.value = 0;

        PersistentDataManager.ReadData("test");
        
        Assert.AreEqual(10, PersistentDataObjectTestClass.Instance.value);
        Assert.AreEqual(50, PersistentDataObjectTestClass2.Instance.value);
    }

    [Test]
    public void PrivateConstructorTest()
    {
        Assert.Throws(typeof(InvalidOperationException),() => { PersistentDataObjectTestClass3.Instance.value = 5; });
    }
}

