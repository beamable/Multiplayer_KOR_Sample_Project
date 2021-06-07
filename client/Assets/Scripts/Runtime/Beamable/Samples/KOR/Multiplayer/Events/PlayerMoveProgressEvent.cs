using UnityEngine;

namespace Beamable.Samples.KOR.Multiplayer.Events
{
   public class PlayerMoveProgressEvent : KOREvent
   {
      //  Properties -----------------------------------
      public uint dirX;
      public uint dirY;

      public uint endTime;

      //  Constructor   --------------------------------
      public PlayerMoveProgressEvent(sfloat time, Vector3 direction)
      {
         // we can't serialize soft floats over the network, so we need to send their raw byte representations instead.
         endTime = time.RawValue;
         dirX = ((sfloat) direction.x).RawValue;
         dirY = ((sfloat) direction.z).RawValue;
      }
   }
}