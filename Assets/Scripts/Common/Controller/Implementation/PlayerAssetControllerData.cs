using System;

namespace Common
{
    public class PlayerAssetControllerData : PersistentDataObject<PlayerAssetControllerData>
    {
        public float credits
        {
            get; set;
        }
    }
}
