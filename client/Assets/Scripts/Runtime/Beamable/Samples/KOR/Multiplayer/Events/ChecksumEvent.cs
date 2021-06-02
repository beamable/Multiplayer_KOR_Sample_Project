namespace Beamable.Samples.KOR.Multiplayer.Events
{
   public class ChecksumEvent : KOREvent
   {
      public long ForTick;
      public string Hash;
      
      public ChecksumEvent(string hash, long forTick)
      {
         Hash = hash;
         ForTick = forTick;
      }
   }
}