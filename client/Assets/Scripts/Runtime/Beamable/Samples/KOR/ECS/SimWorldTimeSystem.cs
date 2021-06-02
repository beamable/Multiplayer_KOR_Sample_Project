using Beamable.Examples.Features.Multiplayer.Core;
using Unity.Core;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Beamable.Samples.KOR.Multiplayer
{
   [Preserve]
   [UpdateInGroup(typeof(InitializationSystemGroup))]
   [DisableAutoCreation]
   public class SimWorldTimeSystem : ComponentSystem
   {
      private const int AllowedPastFrameCount = 5;
      private const int CatchUpFrameCount = 2;
      private const int AllowedFutureFrameCount = 1;

      protected override void OnCreate()
      {
         base.OnCreate();
         World.SetTime(new TimeData(0, 0));
      }

      protected override void OnUpdate()
      {
         var now = World.Time.ElapsedTime;
         var secondsPerFrame = 1 / (float) NetworkController.NetworkFramesPerSecond;
         var simTime = NetworkController.HighestSeenNetworkFrame * secondsPerFrame;
         var maxTime = simTime + secondsPerFrame * AllowedFutureFrameCount; // we can simulate the current network tick into the future.

         var minTime = simTime - secondsPerFrame * AllowedPastFrameCount; // we can sit a bit in the past...

         var normalDelta = UnityEngine.Time.deltaTime;
         var normalNextTime = now + normalDelta;
         if (normalNextTime >= maxTime)
         {
            var fastDelta = maxTime - (float) now; // should equal zero, most of the time.
            // Debug.Log("TOO FAST: " + fastDelta);
            World.SetTime(new TimeData(maxTime, fastDelta ));
         }
         else if (normalNextTime < minTime)
         {
            var catchUpTime = simTime - secondsPerFrame * CatchUpFrameCount;
            var slowDelta = catchUpTime - (float)now;
            // Debug.Log("TOO SLOW: " + normalNextTime + " < " + minTime + " || " + slowDelta);
            World.SetTime(new TimeData(catchUpTime,  slowDelta) );
         }
         else
         {
            World.SetTime(new TimeData(normalNextTime, normalDelta));
         }
      }
   }
}
