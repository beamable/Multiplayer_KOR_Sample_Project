namespace Beamable.Samples.KOR.Multiplayer.Events
{
   public class TickEvent : KOREvent
   {
      public long Tick;

      public TickEvent(long tick)
      {
         Tick = tick;
      }
   }
}