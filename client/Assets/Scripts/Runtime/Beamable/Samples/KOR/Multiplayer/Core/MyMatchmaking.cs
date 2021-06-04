using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Beamable.Common.Content;
using Beamable.Experimental.Api.Matchmaking;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Timer = System.Timers.Timer;

/// <summary>
/// Reusable matchmaking that is not game-specific.
/// </summary>
namespace Beamable.Examples.Features.Multiplayer.Core
{
   [Serializable]
   public class SimGameTypeRef : ContentRef<SimGameType> { }

   /// <summary>
   /// Contains the in-progress matchmaking data. When the process is complete,
   /// this contains the players list and the RoomId
   /// </summary>
   [Serializable]
   public class MyMatchmakingResult
   {
      //  Properties  -------------------------------------
      public bool IsComplete { get { return !string.IsNullOrEmpty(RoomId); } }
      public long LocalPlayerDbid { get { return _localPlayerDbid; } }
      public int TargetPlayerCount { get { return _targetPlayerCount; } }

      //  Fields  -----------------------------------------
      public string RoomId;
      public int SecondsRemaining = 0;
      public List<long> CurrentPlayerDbidList = new List<long>();
      public string ErrorMessage = "";
      //
      private long _localPlayerDbid;
      private int _targetPlayerCount;


      public MyMatchmakingResult(long localPlayerDbid, int targetPlayerCount)
      {
         _localPlayerDbid = localPlayerDbid;
         _targetPlayerCount = targetPlayerCount;
      }


      //  Other Methods  ----------------------------------
      public override string ToString()
      {
         return $"[MyMatchmakingResult (" +
            $"RoomId={RoomId}, " +
            $"TargetPlayerCount={TargetPlayerCount}, " +
            $"players.Count={CurrentPlayerDbidList.Count})]";
      }
   }

   /// <summary>
   /// For the EXAMPLE scene(s). This is a custom implementation of the Matchmaking. This serves as a working template
   /// for real-world use. Feel free to copy this source as inspiration for production games.
   /// </summary>
   public class MyMatchmaking
   {
      //  Events  -----------------------------------------
      public event Action<MyMatchmakingResult> OnProgress;
      public event Action<MyMatchmakingResult> OnComplete;

      //  Properties  -------------------------------------
      public MyMatchmakingResult MyMatchmakingResult { get { return _myMatchmakingResult; } }

      //  Fields  -----------------------------------------
      public const string DefaultRoomId = "DefaultRoom";
      public const int Delay = 1000;

      private MyMatchmakingResult _myMatchmakingResult;
      private MatchmakingService _matchmakingService;
      private SimGameType _simGameType;
      private CancellationTokenSource _matchmakingOngoing;
      private bool _isDebugLog = false;

      public MyMatchmaking(MatchmakingService matchmakingService,
         SimGameType simGameType, long LocalPlayerDbid, bool isDebugLog = false)
      {
         _matchmakingService = matchmakingService;
         _simGameType = simGameType;
         _isDebugLog = isDebugLog;

         _myMatchmakingResult = new MyMatchmakingResult(LocalPlayerDbid, _simGameType.maxPlayers);
      }

      //  Other Methods  ----------------------------------

      /// <summary>
      /// Start the matchmaking process
      /// </summary>
      /// <returns></returns>
      public async Task Start()
      {
         _myMatchmakingResult.RoomId = "";
         _myMatchmakingResult.SecondsRemaining = 0;

         DebugLog($"MyMatchmaking.Start() MinPlayersToStart = {_simGameType.minPlayersToStart.Value}, " +
                  $"TargetPlayerCount = {_simGameType.maxPlayers}");

         var handle = await _matchmakingService.StartMatchmaking(_simGameType.Id);

         var estimatedCompletionTime = Time.realtimeSinceStartup + handle.Status.SecondsRemaining;
         handle.OnUpdate += (update) =>
         {
            estimatedCompletionTime = Time.realtimeSinceStartup + handle.Status.SecondsRemaining;
         };

         handle.OnMatchTimeout += (timeout) =>
         {
            _myMatchmakingResult.ErrorMessage = "Timeout";
         };

         try
         {
            _matchmakingOngoing = new CancellationTokenSource();
            var token = _matchmakingOngoing.Token;
            do
            {
               if (token.IsCancellationRequested) return;

               _myMatchmakingResult.SecondsRemaining = (int) (estimatedCompletionTime - Time.realtimeSinceStartup);
               _myMatchmakingResult.CurrentPlayerDbidList = handle.Status.Players;
               _myMatchmakingResult.RoomId = handle.Status.GameId;
               OnProgress?.Invoke(_myMatchmakingResult);
               await Task.Delay(1000, token);
            } while (handle.State == MatchmakingState.Searching);
         }
         finally
         {
            _matchmakingOngoing.Dispose();
            _matchmakingOngoing = null;
         }

         // Invoke Progress #2
         OnProgress?.Invoke(_myMatchmakingResult);

         // Invoke Complete
         OnComplete?.Invoke(_myMatchmakingResult);
      }

      /// <summary>
      /// Stop the matchmaking process
      /// </summary>
      /// <returns></returns>
      public async void Stop()
      {
         await _matchmakingService.CancelMatchmaking(_simGameType.Id);
         _matchmakingOngoing?.Cancel();
      }

      private void DebugLog(string message)
      {
         if (_isDebugLog)
         {
            Debug.Log(message);
         }
      }
   }
}
