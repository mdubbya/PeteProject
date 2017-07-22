using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common
{
    [Serializable]
    public abstract class PersistentGameOptionDataObject<T> where T : PersistentGameOptionDataObject<T>
    {
        [NonSerialized]
        private static object initLock = new object();

        public static T Instance
        {
            get
            {
                if (PersistentDataManager.GetGameOptionDataObject<T>() == null)
                {
                    CreateInstance();
                }
                return PersistentDataManager.GetGameOptionDataObject<T>();
            }
        }

        private static void CreateInstance()
        {
            lock (initLock)
            {
                if (PersistentDataManager.GetGameOptionDataObject<T>() == null)
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
                    PersistentDataManager.RegisterGameOptionDataObject(_Instance);
                }
            }
        }
    }
}
