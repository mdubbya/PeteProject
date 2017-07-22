using System;
using System.Collections.Generic;
using System.Reflection;

namespace Common
{
    [Serializable]
    public abstract class PersistentSaveSlotDataObject<T> where T : PersistentSaveSlotDataObject<T>
    {        
        [NonSerialized]
        private static object initLock = new object();

        public static T Instance
        {
            get
            {
                if (PersistentDataManager.GetSaveSlotDataObject<T>()==null)
                {
                    CreateInstance();
                }
                return PersistentDataManager.GetSaveSlotDataObject<T>();
            }
        }

        private static void CreateInstance()
        {
            lock (initLock)
            {
                if (PersistentDataManager.GetSaveSlotDataObject<T>()==null)
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
                    PersistentDataManager.RegisterSaveSlotDataObject(_Instance);
                }
            }
        }
    }
}
