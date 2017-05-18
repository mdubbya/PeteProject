using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common
{
    [Serializable]
    public abstract class PersistentDataObject<T> where T : PersistentDataObject<T>
    {        
        [NonSerialized]
        private static object initLock = new object();

        public static T Instance
        {
            get
            {
                if (PersistentDataManager.Get<T>()==null)
                {
                    CreateInstance();
                }
                return PersistentDataManager.Get<T>();
            }
        }

        private static void CreateInstance()
        {
            lock (initLock)
            {
                if (PersistentDataManager.Get<T>()==null)
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
                    PersistentDataManager.RegisterDataObject(_Instance);
                }
            }
        }
    }
}
