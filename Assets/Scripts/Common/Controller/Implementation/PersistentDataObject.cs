using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using TradingMiniGame;
using TradingSelection;

namespace Common
{
    [Serializable]
    public abstract class PersistentDataObject<T> where T : PersistentDataObject<T>
    {
        [NonSerialized]
        private static Dictionary<Type, object> _registeredDataObjects = new Dictionary<Type, object>();

        [NonSerialized]
        private static string fileExtension = ".dat";

        public static void ReadData()
        {
            using (FileStream stream = new FileStream("gamedata" + fileExtension, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                _registeredDataObjects = (Dictionary<Type,object>)formatter.Deserialize(stream);
            }
        }

        public static void WriteData()
        {
            using (FileStream stream = new FileStream("gamedata" + fileExtension, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, _registeredDataObjects);
            }
        }
        
        [NonSerialized]
        private static object initLock = new object();

        public static T Instance
        {
            get
            {
                if (!_registeredDataObjects.ContainsKey(typeof(T)))
                {
                    CreateInstance();
                }
                return (T)_registeredDataObjects[typeof(T)];
            }
        }

        private static void CreateInstance()
        {
            lock (initLock)
            {
                if (!_registeredDataObjects.ContainsKey(typeof(T)))
                {
                    Type t = typeof(T);

                    ConstructorInfo[] constructors = t.GetConstructors();
                    if (constructors.Length > 0)
                    {
                        throw new InvalidOperationException("Type has public constructor.");
                    }

                    // Create an instance via the private constructor
                    var _Instance = (T)Activator.CreateInstance(t, true);
                    // Register instance
                    _registeredDataObjects[t] = _Instance;
                }
            }
        }
    }
}
