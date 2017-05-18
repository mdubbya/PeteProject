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
        private static Dictionary<Type, object> _registeredDataObjects = new Dictionary<Type, object>();
        
        public static void RegisterDataObject<T>(T obj) where T : PersistentDataObject<T>
        {
            _registeredDataObjects[typeof(T)] = obj;
        }

        public static T Get<T>() where T : PersistentDataObject<T>
        {
            return _registeredDataObjects.ContainsKey(typeof(T)) ? (T)_registeredDataObjects[typeof(T)]: null;
        }

        public static void ReadData(string fileName)
        {
            using (FileStream stream = new FileStream(fileName + Constants.SaveSlotExtension, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                _registeredDataObjects = (Dictionary<Type, object>)formatter.Deserialize(stream);
            }
        }

        public static void WriteData(string fileName)
        {
            using (FileStream stream = new FileStream(fileName + Constants.SaveSlotExtension, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, _registeredDataObjects);
            }
        }
    }
}
