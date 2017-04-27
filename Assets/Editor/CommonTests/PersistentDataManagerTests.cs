using NUnit.Framework;
using NSubstitute;
using Common;
using System;

[TestFixture]
public class PersistentDataManagerTests
{
    [Serializable]
    private class SerializableTestClass
    {
        public int value { get; set; }
    }

    private class NonSerializableTestClass
    {
        public int value { get; set; }
    }

    [Test]
    public void ReadWriteDataTest()
    {
        SerializableTestClass test = new SerializableTestClass();
        test.value = 10;
        PersistentDataManager.SetData(test);
        PersistentDataManager.WriteData();
        PersistentDataManager.SetData<SerializableTestClass>(null);
        PersistentDataManager.ReadData();

        Assert.AreEqual(10, PersistentDataManager.GetData<SerializableTestClass>().value);

        
    }
}

