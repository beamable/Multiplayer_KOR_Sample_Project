using Beamable.Samples.KOR.Data;

namespace Beamable.Samples.KOR.Multiplayer.Events
{
    public class ReadyEvent : KOREvent
    {
        public ReadyEvent(Attributes attributes)
        {
            aggregateChargeSpeed = attributes.ChargeSpeed;
            aggregateMovementSpeed = attributes.MovementSpeed;
        }

        public int aggregateChargeSpeed;
        public int aggregateMovementSpeed;
    }
}