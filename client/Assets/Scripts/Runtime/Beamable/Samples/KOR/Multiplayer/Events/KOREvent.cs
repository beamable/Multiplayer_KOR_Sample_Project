using System;
using UnityEngine;

namespace Beamable.Samples.KOR.Multiplayer.Events
{
   //Used to hide some API from common use cases
   public interface IHiddenKOREvent
   {
      /// <summary>
      /// This is set to pack-in WHO SENT the event object.
      /// </summary>
      /// <param name="playerDbid"></param>
      void SetPlayerDbid(long playerDbid);
   }

   /// <summary>
   /// The base event for all multiplayer events in
   /// this sample game project.
   /// </summary>
   [Serializable]
   public class KOREvent: IHiddenKOREvent
   {
      //  Properties -----------------------------------

      /// <summary>
      /// The player who SENT the event.
      /// </summary>
      public long PlayerDbid { get { return _playerDbid; } }

      /// <summary>
      /// Has the event been consumed by the local client, yet? Call <see cref="Consume"/>.
      /// </summary>
      public bool Available { get; private set; } = true; // TODO: refactor "availability" into the sim log as a global consumer

      //  Fields ---------------------------------------
      [SerializeField]
      private long _playerDbid;

      //  Other Methods --------------------------------
      public void SetPlayerDbid(long playerDbid)
      {
         _playerDbid = playerDbid;
      }

      /// <summary>
      /// Marks the event as consumed and unavailable. Use this method to only process an event one time.
      /// </summary>
      public void Consume()
      {
         Available = false;
      }
   }
}