using Beamable.Samples.TBF.Exceptions;
using Beamable.Samples.TBF.Multiplayer.Events;
using System;
using UnityEngine;

namespace Beamable.Samples.TBF.Multiplayer
{
   /// <summary>
   /// This is the AI for the bot if bot mode is enabled (optional).
   /// </summary>
   [Serializable]
   public class RemotePlayerAI
   {
      public enum AIMode
      {
         Production,      // For production
         DebugAlwaysTie,  // For testing
         DebugNeverTie    // For testing
      }

      //  Properties -----------------------------------
      public bool IsEnabled { get { return _isEnabled; } }

      public readonly long RemotePlayerDbid = -999999999; //conspicous, fake dbid for production usage

      //  Fields ---------------------------------------
      private bool _isEnabled = false;

      /// <summary>
      /// A custom Random is used for deterministic results for all
      /// players. This is a best practice for Beamable Multplayer.
      /// </summary>
      private System.Random _random;

      //  Other Methods --------------------------------
      public RemotePlayerAI (System.Random random, bool isEnabled)
      {
         _random = random;
         _isEnabled = isEnabled;
      }


      public GameMoveEvent GetNextRemoteGameMoveEvent(GameMoveType localGameMoveType)
      {
         GameMoveType remotePlayerAIGameMoveType = GetNextRemoteGameMoveType(localGameMoveType);
         GameMoveEvent remotePlayerAIGameMoveEvent = new GameMoveEvent(remotePlayerAIGameMoveType);
         ((IHiddenTBFEvent)remotePlayerAIGameMoveEvent).SetPlayerDbid(RemotePlayerDbid);
         
         return remotePlayerAIGameMoveEvent;
      }


      private GameMoveType GetNextRemoteGameMoveType(GameMoveType localGameMoveType)
      {

         GameMoveType gameMoveType = GameMoveType.Null;

         //The AI has optional debug modes for testing 
         switch (TBFConstants.AIMode)
         {
            case RemotePlayerAI.AIMode.Production:

               //Typical, for production usage
               gameMoveType = GetRandomRemoteGameMoveType();
               break;

            case RemotePlayerAI.AIMode.DebugAlwaysTie:

               gameMoveType = localGameMoveType;

               DebugLog($"[Debug] RemotePlayerAIDebugMode={TBFConstants.AIMode}");
               break;

            case RemotePlayerAI.AIMode.DebugNeverTie:

               do
               {
                  gameMoveType = GetRandomRemoteGameMoveType();
               } while (gameMoveType == localGameMoveType);

               DebugLog($"[Debug] RemotePlayerAIDebugMode={TBFConstants.AIMode}");
               break;

            default:
               SwitchDefaultException.Throw(TBFConstants.AIMode);
               break;
         }

         return gameMoveType;
      }

      private GameMoveType GetRandomRemoteGameMoveType()
      {
         GameMoveType gameMoveType = GameMoveType.Null;

         // Values of 1/2/3
         int index = _random.Next(1, 4);

         switch (index)
         {
            case 1:
               gameMoveType = GameMoveType.High;
               break;
            case 2:
               gameMoveType = GameMoveType.Mid;
               break;
            case 3:
               gameMoveType = GameMoveType.Low;
               break;
            default:
               SwitchDefaultException.Throw(index);
               break;
         }

         DebugLog($"RemotePlayerAI.GetNextGameMoveType() {gameMoveType}");
         return gameMoveType;
      }

      private void DebugLog(string message)
      {
         if (TBFConstants.IsDebugLogging)
         {
            Debug.Log(message);
         }
      }
   }
}
