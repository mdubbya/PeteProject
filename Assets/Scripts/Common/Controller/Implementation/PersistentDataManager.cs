using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common
{
    public static class PersistentDataManager
    {
        [NonSerialized]
        private static Dictionary<Type, object> _registeredSaveSlotDataObjects = new Dictionary<Type, object>();
        [NonSerialized]
        private static Dictionary<Type, object> _registeredGameOptionDataObjects = new Dictionary<Type, object>();

        public static void RegisterSaveSlotDataObject<T>(T obj) where T : PersistentSaveSlotDataObject<T>
        {
            _registeredSaveSlotDataObjects[typeof(T)] = obj;
        }


        public static void RegisterGameOptionDataObject<T>(T obj) where T : PersistentGameOptionDataObject<T>
        {
            _registeredGameOptionDataObjects[typeof(T)] = obj;
        }


        public static T GetSaveSlotDataObject<T>() where T : PersistentSaveSlotDataObject<T>
        {
            return _registeredSaveSlotDataObjects.ContainsKey(typeof(T)) ? (T)_registeredSaveSlotDataObjects[typeof(T)] : null;
        }

        public static T GetGameOptionDataObject<T>() where T : PersistentGameOptionDataObject<T>
        {
            return _registeredGameOptionDataObjects.ContainsKey(typeof(T)) ? (T)_registeredGameOptionDataObjects[typeof(T)] : null;
        }


        private static void WriteData(string fullFilePath, object objectToWrite)
        {
            using (FileStream stream = new FileStream(fullFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, objectToWrite);
            }
        }


        private static object ReadData(string fullFilePath)
        {
            using (FileStream stream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }


        public static void ReadSaveSlotData(string baseFileName)
        {
            _registeredSaveSlotDataObjects = (Dictionary<Type, object>)ReadData(baseFileName + Constants.SaveSlotExtension);
        }


        public static void WriteSaveSlotData(string baseFileName)
        {
            WriteData(baseFileName + Constants.SaveSlotExtension, _registeredSaveSlotDataObjects);
        }


        public static void WriteGameOptionData()
        {
            WriteData(Constants.GameOptionsFileName, _registeredGameOptionDataObjects);
        }


        public static void ReadGameOptionData()
        {
            _registeredGameOptionDataObjects = (Dictionary<Type, object>)ReadData(Constants.GameOptionsFileName);
        }
    }
}
