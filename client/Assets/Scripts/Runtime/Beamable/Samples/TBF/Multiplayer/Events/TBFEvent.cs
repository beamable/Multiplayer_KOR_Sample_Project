using System;
using UnityEngine;

namespace Beamable.Samples.TBF.Multiplayer.Events
{
   //Used to hide some API from common use cases
   public interface IHiddenTBFEvent
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
   public class TBFEvent: IHiddenTBFEvent
   {
      //  Properties -----------------------------------

      /// <summary>
      /// The player who SENT the event.
      /// </summary>
      public long PlayerDbid { get { return _playerDbid; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private long _playerDbid;

      //  Other Methods --------------------------------
      public void SetPlayerDbid(long playerDbid)
      {
         _playerDbid = playerDbid;
      }
   }
}