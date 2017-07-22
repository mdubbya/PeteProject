using System;

namespace Common
{
    public class PlayerAssetControllerData : PersistentSaveSlotDataObject<PlayerAssetControllerData>
    {
        public float credits
        {
            get; set;
        }
    }
}
