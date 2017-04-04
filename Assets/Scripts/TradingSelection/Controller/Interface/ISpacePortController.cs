
namespace TradingSelection
{
    public interface ISpacePortController
    {
        void EstablishTradeLane(ISpacePortController otherPort);
        void CancelTradeLane(ISpacePortController otherPort);
        bool TradeLaneEstablished(ISpacePortController otherPort);
    }
}
