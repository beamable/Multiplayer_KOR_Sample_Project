using Beamable.Examples.Features.Multiplayer.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityS.Mathematics;
using UnityS.Physics;
using UnityS.Physics.Systems;

namespace Beamable.Samples.KOR.Multiplayer
{
   [UpdateAfter(typeof(EndFramePhysicsSystem))]
   public class PlayerBounceSystem : JobComponentSystem
   {
      private BuildPhysicsWorld _buildPhysicsWorld;
      private StepPhysicsWorld _stepPhysicsWorld;

      protected override void OnCreate()
      {
         base.OnCreate();
         _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
         _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
      }

      protected override JobHandle OnUpdate(JobHandle inputDeps)
      {
         var job = new PlayerBounceSystemJob();
         job.bouncyGroup = GetComponentDataFromEntity<BouncyTag>(true);
         job.impulseGroup = GetComponentDataFromEntity<PhysicsImpulse>();
         job.velocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(true);

         var jobHandle = job.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
         jobHandle.Complete();

         return jobHandle;
      }


      struct PlayerBounceSystemJob : ICollisionEventsJob
      {
         [ReadOnly]
         public ComponentDataFromEntity<BouncyTag> bouncyGroup;

         [ReadOnly]
         public ComponentDataFromEntity<PhysicsVelocity> velocityGroup;

         public ComponentDataFromEntity<PhysicsImpulse> impulseGroup;


         public void Execute(CollisionEvent collisionEvent)
         {

            var a = collisionEvent.EntityA;
            var b = collisionEvent.EntityB;

            var aIsBouncy = bouncyGroup.HasComponent(a);
            var bIsBouncy = bouncyGroup.HasComponent(b);
            var aHasImpulse = impulseGroup.HasComponent(a);
            var bHasImpulse = impulseGroup.HasComponent(b);

            if (aIsBouncy && bHasImpulse)
            {
               // apply an explosion impulse to b.
               var bImpulse = impulseGroup[b];
               bImpulse.Impulse = collisionEvent.Normal * bouncyGroup[a].Bounce;
               impulseGroup[b] = bImpulse;
            }

            if (bIsBouncy && aHasImpulse)
            {
               // apply an explosion impulse to a
               var aImpulse = impulseGroup[a];
               aImpulse.Impulse = collisionEvent.Normal * bouncyGroup[b].Bounce;
               impulseGroup[a] = aImpulse;
            }

            // if (aIsBouncy && bHasImpluse && bIsBouncy && aHasImpulse)
            // {
            //    // apply an explosion impulse to both a and b.
            //    var aImpulse = impulseGroup[a];
            //    aImpulse.Impulse = collisionEvent.Normal * 10;
            //    impulseGroup[a] = aImpulse;
            //    var bImpulse = impulseGroup[b];
            //    bImpulse.Impulse = collisionEvent.Normal * 10;
            //    impulseGroup[b] = bImpulse;
            // }
         }
      }
   }
}