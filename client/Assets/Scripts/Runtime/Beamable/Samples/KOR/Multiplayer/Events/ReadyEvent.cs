using Beamable.Samples.KOR.Data;

namespace Beamable.Samples.KOR.Multiplayer.Events
{
    public class ReadyEvent : KOREvent
    {
        public ReadyEvent(Attributes attributes, string alias)
        {
            aggregateChargeSpeed = attributes.ChargeSpeed;
            aggregateMovementSpeed = attributes.MovementSpeed;
            playerAlias = alias;
        }

        public int aggregateChargeSpeed;
        public int aggregateMovementSpeed;
        public string playerAlias;
    }
}