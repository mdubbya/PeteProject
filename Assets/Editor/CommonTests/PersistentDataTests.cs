using NUnit.Framework;
using NSubstitute;
using Common;
using System;

[TestFixture]
public class PersistentDataTests
{
    [Serializable]
    public class PersistentSaveSlotDataObjectTestClass : PersistentSaveSlotDataObject<PersistentSaveSlotDataObjectTestClass>
    {
        public int value { get; set; }
        private PersistentSaveSlotDataObjectTestClass() { }
    }

    [Serializable]
    public class PersistentGameOptionDataObjectTestClass : PersistentGameOptionDataObject<PersistentGameOptionDataObjectTestClass>
    {
        public int value { get; set; }
        private PersistentGameOptionDataObjectTestClass() { }
    }

    [Serializable]
    public class PersistentSaveSlotDataObjectTestClass2 : PersistentSaveSlotDataObject<PersistentSaveSlotDataObjectTestClass2>
    {
        public int value { get; set; }
    }

    [Serializable]
    public class PersistentGameOptionDataObjectTestClas2 : PersistentGameOptionDataObject<PersistentGameOptionDataObjectTestClas2>
    {
        public int value { get; set; }
    }

    [Test]
    public void ReadWriteSaveSlotDataTest()
    {
        PersistentSaveSlotDataObjectTestClass.Instance.value = 10;
        PersistentDataManager.WriteSaveSlotData("test");

        PersistentSaveSlotDataObjectTestClass.Instance.value = 0;

        Assert.AreEqual(0, PersistentSaveSlotDataObjectTestClass.Instance.value);

        PersistentDataManager.ReadSaveSlotData("test");

        Assert.AreEqual(10, PersistentSaveSlotDataObjectTestClass.Instance.value);

    }

    [Test]
    public void ReadWriteGameOptionDataTest()
    {
        PersistentGameOptionDataObjectTestClass.Instance.value = 10;
        PersistentDataManager.WriteGameOptionData();

        PersistentGameOptionDataObjectTestClass.Instance.value = 0;

        Assert.AreEqual(0, PersistentGameOptionDataObjectTestClass.Instance.value);

        PersistentDataManager.ReadGameOptionData();

        Assert.AreEqual(10, PersistentGameOptionDataObjectTestClass.Instance.value);
    }

    [Test]
    public void PrivateConstructorTest()
    {
        Assert.Throws(typeof(InvalidOperationException),() => { PersistentSaveSlotDataObjectTestClass2.Instance.value = 5; });
        Assert.Throws(typeof(InvalidOperationException), () => { PersistentGameOptionDataObjectTestClas2.Instance.value = 5; });
    }
}

