using System;
using UnityEngine;

namespace Beamable.Samples.KOR.Multiplayer.Events
{
   [Serializable]
   public class PlayerMoveStartedEvent : KOREvent
   {
      //  Properties -----------------------------------
      public uint dirX;
      public uint dirY;

      public uint startTime;

      //  Constructor   --------------------------------
      public PlayerMoveStartedEvent(sfloat time, Vector3 direction)
      {
         // we can't serialize soft floats over the network, so we need to send their raw byte representations instead.
         startTime = time.RawValue;
         dirX = ((sfloat) direction.x).RawValue;
         dirY = ((sfloat) direction.z).RawValue;
      }
   }
}