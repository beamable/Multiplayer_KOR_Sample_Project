using System;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.Core;
using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
   public class GameTimerBehaviour : MonoBehaviour
   {
      public sfloat durationSeconds = (sfloat)60;

      public int SecondsRemaining { get; private set; }

      public event Action OnGameOver;

      private sfloat startTime;
      private sfloat endTime;
      private sfloat currTime;
      private bool started;
      private bool ended;

      public void StartMatch()
      {
         var tick = NetworkController.HighestSeenNetworkFrame;
         var time = (sfloat)tick / (sfloat) NetworkController.NetworkFramesPerSecond;
         endTime = time + durationSeconds;
         started = true;
         SecondsRemaining = (int)durationSeconds;
         NetworkController.Instance.Log.CreateNewConsumer(NetworkUpdate);
      }

      private void NetworkUpdate(TimeUpdate update)
      {
         if (ended) return;

         currTime = (sfloat)update.ElapsedTime;
         ended = currTime >= endTime;
         SecondsRemaining = (int) (endTime - currTime);

         if (ended)
         {
            HandleEnd();
         }
      }

      private void HandleEnd()
      {
         OnGameOver?.Invoke();
      }
   }
}