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
      private NetworkController _networkController;
      private int _framesPerSecond;
      protected override void OnCreate()
      {
         _networkController = NetworkController.Instance;
         _framesPerSecond = NetworkController.NetworkFramesPerSecond;
         base.OnCreate();
      }

      protected override void OnUpdate()
      {
         var time = World.Time.ElapsedTime;
         var frame = (long) (time * _framesPerSecond);

         _networkController.Log.NotifyConsumers(frame, (float)time, (float)World.Time.DeltaTime);
      }
   }
}