using System;
using System.Collections.Generic;
using System.Linq;
using Beamable.Samples.KOR.Multiplayer.Events;
using UnityEngine;

namespace Beamable.Examples.Features.Multiplayer.Core
{
   public class SimulationLog
   {
      private Dictionary<long, List<KOREvent>> _tickToMessages = new Dictionary<long, List<KOREvent>>();
      private long _highestTick = 0;
      private int _nextConsumerId;
      private Dictionary<int, Action<TimeUpdate>> _consumerIdToUpdater = new Dictionary<int, Action<TimeUpdate>>();

      private Dictionary<int, long> _consumerIdToTick = new Dictionary<int, long>();
      private Dictionary<long, string> _tickToHash = new Dictionary<long, string>();

      private Dictionary<long, string> _pendingHashValidations = new Dictionary<long, string>();

      private long latestInvalidFrame = -1;
      private long latestValidFrame = -1;

      public bool HasHashForTick(long tick)
      {
         return _tickToHash.ContainsKey(tick);
      }

      public bool TryGetInvalidHashTick(out long tick)
      {
         tick = latestInvalidFrame;
         return tick > -1;
      }

      public bool TryGetValidHashTick(out long tick)
      {
         tick = latestValidFrame;
         return tick > -1;
      }

      public string GetHashForTick(long tick)
      {
         if (!HasHashForTick(tick))
         {
            throw new Exception("No hash has been calculated for tick " + tick);
         }

         return _tickToHash[tick];
      }

      public void ReportHashForTick(long tick, string hash)
      {
         _tickToHash[tick] = hash;
         if (_pendingHashValidations.TryGetValue(tick, out var pendingAssertHash))
         {
            AssertHash(tick, pendingAssertHash);
            _pendingHashValidations.Remove(tick);
         }
      }

      public void EnqueueHashAssertion(long tick, string hash)
      {
         if (!HasHashForTick(tick))
         {
            if (_pendingHashValidations.TryGetValue(tick, out var existingHash) && !Equals(existingHash, hash))
            {
               Debug.LogError("QUEUED HASH MISMATCH!!! FOR TICK " + tick);
               latestInvalidFrame = tick;
            }

            _pendingHashValidations.Add(tick, hash);
            return;
         }

         AssertHash(tick, hash);
      }

      public bool AssertHash(long tick, string hash)
      {
         var actualHash = _tickToHash[tick];
         if (!Equals(actualHash, hash))
         {
            Debug.LogError("HASH MISMATCH!!! FOR TICK " + tick);
            latestInvalidFrame = tick;
            return false;
         }
         else
         {
            Debug.Log("Hash pass for tick: " + tick);
            latestValidFrame = tick;
            return true;
         }
      }

      public IEnumerable<KOREvent> GetMessagesForTick(long dbid, long tick)
      {
         return GetMessagesForTick(tick).Where(message => message.PlayerDbid == dbid);
      }

      public IEnumerable<T> GetMessagesForTick<T>(long dbid, long tick)
         where T : KOREvent
      {
         return GetMessagesForTick(tick).Where(message => message.PlayerDbid == dbid && message is T).Cast<T>();
      }

      public IEnumerable<T> GetMessagesForTick<T>(long tick)
         where T : KOREvent
      {
         return GetMessagesForTick(tick).Where(message => message is T).Cast<T>();
      }

      public int CreateNewConsumer(Action<TimeUpdate> onUpdate, long startTick = -1)
      {
         _nextConsumerId++;
         _consumerIdToTick.Add(_nextConsumerId, startTick);
         _consumerIdToUpdater.Add(_nextConsumerId, onUpdate);
         return _nextConsumerId;
      }

      public void NotifyConsumers(long tick, float elapsedTime, float deltaTime)
      {
         foreach (var kvp in _consumerIdToUpdater)
         {
            var consumerId = kvp.Key;
            var updateFunction = kvp.Value;
            var messages = GetMessagesForTick(tick, consumerId).ToList();

            var update = new TimeUpdate
            {
               Tick = tick,
               ElapsedTime = elapsedTime,
               DeltaTime = deltaTime,
               Events = messages
            };

            updateFunction(update);
         }
      }

      public IEnumerable<KOREvent> GetMessagesForTick(long tick)
      {
         if (!_tickToMessages.ContainsKey(tick))
         {
            yield break;
         }

         var messages = _tickToMessages[tick];
         foreach (var message in messages)
         {
            yield return message;
         }
      }

      public IEnumerable<KOREvent> GetMessagesForTick(long tick, int consumerId)
      {
         if (!_consumerIdToTick.TryGetValue(consumerId, out var lastTick))
         {
            lastTick = -1;
         }

         if (tick > lastTick)
         {
            _consumerIdToTick[consumerId] = tick;
            foreach (var message in GetMessagesForTick(tick))
            {
               yield return message;
            }
         }
      }

      public IEnumerable<TimeUpdate> GetTimeUpdates(int consumerId)
      {
         var tick = NetworkController.HighestSeenNetworkFrame;
         var seenTick = _consumerIdToTick[consumerId];
         var timestep = 1f / NetworkController.NetworkFramesPerSecond;

         for (var t = seenTick + 1; t <= tick; t++)
         {
            var elapsedTime = t / (float) NetworkController.NetworkFramesPerSecond;

            var messages = GetMessagesForTick(t, consumerId).ToList();
            yield return new TimeUpdate
            {
               ElapsedTime = elapsedTime,
               DeltaTime = timestep,
               Events = messages
            };
         }
      }

      public void RecordEvent(KOREvent message)
      {
         Debug.Log("Adding " + message.GetType().Name + " at " + _highestTick + " from " + message.PlayerDbid);
         RecordEvent(_highestTick, message);
      }

      public void RecordEvent(long tick, KOREvent message)
      {
         _highestTick = tick > _highestTick
            ? tick
            : _highestTick;
         if (!_tickToMessages.ContainsKey(tick))
         {
            _tickToMessages.Add(tick, new List<KOREvent>());
         }

         var messages = _tickToMessages[tick];
         messages.Add(message);
      }
   }

   public class TimeUpdate
   {
      public long Tick;
      public float ElapsedTime;
      public float DeltaTime;
      public List<KOREvent> Events;
   }
}