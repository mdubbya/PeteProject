using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TradingMiniGame;
using TradingSelection;

namespace Common
{
    public static class PersistentDataManager
    {
        private static Dictionary<Type, object> _objectDictionary = new Dictionary<Type, object>();

        private static string fileExtension = ".dat";

        public static T GetData<T>()
        {
            return (T)_objectDictionary[typeof(T)];
        }


        public static void SetData<T>(T item)
        {
            if (typeof(T).IsSerializable)
            {
                _objectDictionary[typeof(T)] = item;
            }
            else
            {
                throw new NotSupportedException("object not serializable");
            }
        }


        public static void ReadData()
        {
            Dictionary<Type, object> changes = new Dictionary<Type, object>();
            foreach(var pair in _objectDictionary)
            {
                using (FileStream stream = new FileStream(pair.Key.GetType().FullName + fileExtension, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    changes[pair.Key] = formatter.Deserialize(stream);
                }
            }
            foreach(var pair in changes)
            {
                _objectDictionary[pair.Key] = pair.Value;
            }
        }

        public static void WriteData()
        {
            foreach (var pair in _objectDictionary)
            {
                using (FileStream stream = new FileStream(pair.Key.GetType().FullName + fileExtension, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, pair.Value);
                }
            }
        }
    }
}
