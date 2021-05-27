using Beamable.Samples.TBF.Multiplayer.Events;
using System;
using System.Collections.Generic;
using Beamable.Experimental.Api.Sim;
using UnityEngine;

namespace Beamable.Samples.TBF.Multiplayer
{
   public class TBFMultiplayerSession
   {
      //  Events -------------------------------------------
      public event SimClient.EventCallback<System.Random> OnInit;
      public event SimClient.EventCallback<long> OnConnect;
      public event SimClient.EventCallback<long> OnDisconnect;

      //  Fields  -----------------------------------------
      public System.Random Random { get { return _random; } }
      public int PlayerDbidsCount { get { return _playerDbids.Count; } }
      public int TargetPlayerCount { get { return _targetPlayerCount; } }
      public bool IsHumanVsBotMode { get { return PlayerDbidsCount == 1; } }
      public bool IsLocalPlayerDbid(long dbid) { return dbid == _localPlayerDbid; }


      /// <summary>
      /// Determines if events objects are transfered with fullly qualified event names.
      /// True, is more correct.
      /// False, is easier debug logging.
      /// </summary>
      private static bool IsNamespaceSensitive = false;
      private const long FramesPerSecond = 20;
      private const long TargetNetworkLead = 4;
      private int _sessionSeed;
      private System.Random _random;
      private List<long> _playerDbids = new List<long>();
      private SimClient _simClient;
      private long _currentFrame;
      private long _localPlayerDbid;
      private string _roomId;
      private int _targetPlayerCount;

      //  Constructor   --------------------------------
      public TBFMultiplayerSession(long localPlayerDbid, int targetPlayerCount, string roomId)
      {
         _roomId = roomId;
         _localPlayerDbid = localPlayerDbid;
         _targetPlayerCount = targetPlayerCount;
      }

      //  Other Methods   ------------------------------

      /// <summary>
      /// Initialize the <see cref="SimClient"/>.
      /// </summary>
      public void Initialize()
      {
         // Create Multiplayer Session
         _simClient = new SimClient(new SimNetworkEventStream(_roomId),
            FramesPerSecond, TargetNetworkLead);

         // Handle Common Events
         _simClient.OnInit(SimClient_OnInit);
         _simClient.OnConnect(SimClient_OnConnect);
         _simClient.OnDisconnect(SimClient_OnDisconnect);
         _simClient.OnTick(SimClient_OnTick);
      }

      public bool HasPlayerDbidForIndex(int index)
      {
         return index < _playerDbids.Count;
      }

      public long GetPlayerDbidForIndex(int index)
      {
         if (HasPlayerDbidForIndex(index))
         {
            return _playerDbids[index];
         }
         return TBFConstants.UnsetValue;
      }

      /// <summary>
      /// Convenience. Wrap <see cref="SimClient"/> method.
      /// </summary>
      public void Update()
      {
         if (_simClient != null)
         {
            _simClient.Update();
         }

         string message = "";
         message += $"Room: {_roomId}\n";
         message += $"Seed: {_sessionSeed}\n";
         message += $"Frame: {_currentFrame}\n";
         message += $"Dbids:";
         foreach (var dbid in _playerDbids)
         {
            message += $"{dbid},";
         }
         //DebugLog($"message:{message}");
      }


      /// <summary>
      /// Convenience. Wrap <see cref="SimClient"/> method.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="callback"></param>
      /// <returns></returns>
      public SimClient.EventCallback<string> On<T>(string origin, SimClient.EventCallback<T> callback) where T : TBFEvent
      {
         string name = GetEventName<T>();
         DebugLog($"SimClient_On(): {name}");
         return _simClient.On<T>(GetEventName<T>(), origin, callback);
      }

      /// <summary>
      /// Convenience. Wrap <see cref="SimClient"/> method.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="multiplayerSession_OnGameStartEvent"></param>
      public void Remove<T>(SimClient.EventCallback<T> callback)
      {
         //FIX: Beamable API does not yet support EventCallback<T>,
         //so the following casting is required. To be fixed. Known issue.
         _simClient.Remove(callback as SimClient.EventCallback<string>);
      }

      /// <summary>
      /// Convenience. Wrap <see cref="SimClient"/> method.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="evt"></param>
      public void SendEvent<T>(T evt) where T : TBFEvent
      {
         string name = GetEventName<T>();

         //Pack in the local player to EVERY event
         //Uses non-public API to reduce complexity during common use cases
         (evt as IHiddenTBFEvent).SetPlayerDbid(_localPlayerDbid);

         DebugLog($"SimClient_SendEvent(): name={name}, evt.PlayerDbid={evt.PlayerDbid }.");
         _simClient.SendEvent(name, evt);
      }

      //  Private Methods  -----------------------------
      private void DebugLog(string message)
      {
         if (TBFConstants.IsDebugLogging)
         {
            Debug.Log(message);
         }
      }


      /// <summary>
      /// Convert <see cref="TBFEvent"/> name subclass
      /// for server transfer.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <returns></returns>
      private string GetEventName<T>() where T : TBFEvent
      {
         if (IsNamespaceSensitive)
         {
            return typeof(T).FullName.ToString();
         }
         else
         {
            return typeof(T).Name.ToString();
         }
      }


      //  Event Handlers  ------------------------------
      private void SimClient_OnInit(string sessionSeed)
      {
         //Incoming long sessionSeed my be too large for the required int. Convert.
         long sessionSeedLong = long.Parse(sessionSeed);
         float sessionSeedFloat = Mathf.Min(Int32.MaxValue, sessionSeedLong);
         _sessionSeed = (int)sessionSeedLong;

         DebugLog($"SimClient_OnInit() RoomId '{_roomId}' SessionSeed '{_sessionSeed}'");

         _random = new System.Random(_sessionSeed);
         OnInit?.Invoke(_random);
      }


      private void SimClient_OnConnect(string dbid)
      {
         _playerDbids.Add(long.Parse(dbid));
         DebugLog($"SimClient_OnConnect(): {long.Parse(dbid)}");
         OnConnect?.Invoke(long.Parse(dbid));
      }


      private void SimClient_OnDisconnect(string dbid)
      {
         _playerDbids.Remove(long.Parse(dbid));
         DebugLog($"SimClient_OnDisconnect(): {long.Parse(dbid)}");
         OnDisconnect?.Invoke(long.Parse(dbid));
      }

      private void SimClient_OnTick(long currentFrame)
      {
         _currentFrame = currentFrame;
      }
   }
}
