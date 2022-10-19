using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Beamable.Common.Dependencies;
using Beamable.Experimental.Api.Sim;

namespace Beamable.Examples.Features.Multiplayer.Core
{
   public class FastNetworkEventStream : SimNetworkInterface
   {
      private static long REQ_FREQ_MS = (1000 / 20) + 20;

      public string ClientId { get; private set; }
      public bool Ready { get; private set; }
      public bool IsFaulted => simException != null;
      public event Action<SimFaultResult> OnErrorStarted;
      public event Action<SimErrorReport> OnErrorRecovered;
      public event Action<SimFaultResult> OnErrorFailed;
      private List<SimEvent> _eventQueue = new List<SimEvent>();
      private List<SimFrame> _syncFrames = new List<SimFrame>();
      private List<SimFrame> _emptyFrames = new List<SimFrame>();
      private SimFrame _nextFrame = new SimFrame(-1, new List<SimEvent>());
      private long _lastReqTime;
      private bool hasData = false;
      private string roomName;
      private SimNetworkErrorException simException = null;
      private readonly IDependencyProvider _provider;
      private GameRelayService GameRelay => _provider.GetService<GameRelayService>();

      public FastNetworkEventStream(string roomName, string id, IDependencyProvider provider)
      {
         this.roomName = roomName;
         _provider = provider;
         ClientId = id;
         _syncFrames.Add(_nextFrame);
         Ready = true;
      }

      public List<SimFrame> Tick(long curFrame, long maxFrame, long expectedMaxFrame)
      {
         if (IsFaulted)
         {
            throw simException;
         }
         long now = GetTimeMs();
         if ((now - _lastReqTime) >= REQ_FREQ_MS)
         {
            _lastReqTime = now;
            var req = new GameRelaySyncMsg
            {
               t = _nextFrame.Frame
            };
            for (int i = 0; i < _eventQueue.Count; i++)
            {
               var evt = new GameRelayEvent();
               evt.FromSimEvent(_eventQueue[i]);
               req.events.Add(evt);
            }

            _eventQueue.Clear();
            var syncReq = GameRelay.Sync(roomName, req);
               
            syncReq.Then(rsp =>
            {
               if (rsp.t == -1)
               {
                  return;
               }

               // Purge all events we already might know about
               for (int j = 0; j < rsp.events.Count; j++)
               {
                  var evt = rsp.events[j];
                  if (evt.t > _nextFrame.Frame)
                  {
                     _nextFrame.Events.Add(evt.ToSimEvent());
                  }
               }

               // If the response is higher than what we know, let's get it
               if (rsp.t > _nextFrame.Frame)
               {
                  _nextFrame.Frame = rsp.t;
                  hasData = true;
               }
            });
         }

         if (hasData)
         {
            hasData = false;
            return _syncFrames;
         }

         _nextFrame.Events.Clear();
         return _emptyFrames;
      }

      private long GetTimeMs()
      {
         return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
      }

      public void SendEvent(SimEvent evt)
      {
         _eventQueue.Add(evt);
      }
   }
}