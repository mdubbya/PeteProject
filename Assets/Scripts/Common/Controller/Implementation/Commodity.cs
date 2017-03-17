
namespace Common
{
    public enum CommodityType
    {
        //This is by no means set in stone, just some ideas at the moment.
        //TODO: finalize
        Uranium,
        Gold,
        Helium3,
        Plasteel,
        Textiles,
        Diamonds
    }
    

    public class Commodity
    {
        public readonly CommodityType commodityType;
        public readonly float value;
        public readonly ISpacePortController location;

        public Commodity(CommodityType commodityType, float value, ISpacePortController location)
        {
            this.commodityType = commodityType;
            this.value = value;
            this.location = location;
        }
    }
    
}
