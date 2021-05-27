using Beamable.Common.Content;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Experimental.Api.Matchmaking;

namespace Beamable.Samples.TBF.Multiplayer
{
   /// <summary>
   /// This is for the SAMPLE PROJECT Scene(s). For this project the needs are so similar to the
   /// EXAMPLE Scenes, that the EXAMPLE <see cref="MyMatchmaking"/> is extended.
   ///
   /// NOTE: For your production uses, simply copy <see cref="MyMatchmaking"/> as inspiration
   /// and create a new custom class.
   /// </summary>
   public class TBFMatchmaking : MyMatchmaking
   {
      /// <summary>
      /// During development, if the game scene is loaded directly (and thus no matchmaking)
      /// this method is used to give a RoomId. Why random? So that each connection is fresh
      /// and has no history. Otherwise a new connection (within 10-15 seconds of the last connection)
      /// may remember the 'old' session and contain 'old' events.
      /// </summary>
      /// <returns></returns>
      public static string GetRandomRoomId()
      {
         return "TBFRoomId" + string.Format("{00:00}", UnityEngine.Random.Range(0, 1000));
      }

      public TBFMatchmaking(MatchmakingService matchmakingService, SimGameType simGameType, long LocalPlayerDbid) :
         base(matchmakingService, simGameType, LocalPlayerDbid)
      {
      }
   }
}
