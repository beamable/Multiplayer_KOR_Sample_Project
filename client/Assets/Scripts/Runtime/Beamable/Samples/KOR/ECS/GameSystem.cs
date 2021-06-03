using System.Linq;
using Beamable.Examples.Features.Multiplayer.Core;
using Unity.Entities;
using UnityS.Physics.Systems;

namespace Beamable.Samples.KOR.Multiplayer
{

   [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
   [UpdateAfter(typeof(ExportPhysicsWorld))]
   [DisableAutoCreation]
   public class GameSystem : SystemBase
   {
      protected override void OnUpdate()
      {
         // get the tick this represents...
         var network = NetworkController.Instance;
         var time = World.Time.ElapsedTime;
         var frame = (long) (time * NetworkController.NetworkFramesPerSecond);

         var messages = network.Log.GetMessagesForTick(frame).ToList();

         network.Log.NotifyConsumers(frame, (float)time, (float)World.Time.DeltaTime);
         //
         // foreach (var message in messages)
         // {
         //    GameSceneManager.Instance.HandleNetworkEvent(message);
         // }
      }
   }
}