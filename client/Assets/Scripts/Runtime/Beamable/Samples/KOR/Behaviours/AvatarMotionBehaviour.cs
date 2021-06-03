using System;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.Core;
using Beamable.Samples.KOR.Multiplayer;
using Beamable.Samples.KOR.Multiplayer.Events;
using Beamable.Samples.KOR.Views;
using Unity.Entities;
using UnityEngine;
using UnityS.Mathematics;
using UnityS.Physics;
using UnityS.Physics.Extensions;
using UnityS.Transforms;

namespace Beamable.Samples.KOR.Behaviours
{
   public class AvatarMotionBehaviour : MonoBehaviour
   {
      public AvatarView AvatarView;

      public ConvertToNetworkedPhysics NetworkedPhysics;

      public int MaxPower = 22;
      public int MinPower = 8;

      public float PowerGrowthSlope = 10;
      public float PowerGrowthExp = .9f;

      [ReadOnly]
      public int consumerId;

      [ReadOnly]
      public PlayerMoveStartedEvent startEvt;

      private void Start()
      {
         consumerId = NetworkController.Instance.Log.CreateNewConsumer(OnNetworkUpdate);
      }

      public sfloat GetPowerForDeltaTime(sfloat dt)
      {
         var power = dt * (sfloat) PowerGrowthSlope;
         power = math.pow(power, (sfloat) PowerGrowthExp);
         var minPower = (sfloat) MinPower;
         var maxPower = (sfloat) MaxPower;
         if (power < minPower)
         {
            power = minPower;
         } else if (power > maxPower)
         {
            power = maxPower;
         }

         return power;
      }

      public sfloat GetPowerRatioForDeltaTime(sfloat dt)
      {
         var power = GetPowerForDeltaTime(dt);
         var minPower = (sfloat) MinPower;
         var maxPower = (sfloat) MaxPower;
         var ratio = (power - minPower) / (maxPower - minPower);
         return ratio;
      }

      void OnNetworkUpdate(TimeUpdate timeUpdate)
      {
         var tick = (long) (timeUpdate.ElapsedTime * NetworkController.NetworkFramesPerSecond);
         foreach (var message in NetworkController.Instance.Log.GetMessagesForTick(tick, consumerId))
         {
            if (message.PlayerDbid != AvatarView.playerDbid) continue; // this message doesn't belong to the player we care about

            switch (message)
            {
               case PlayerMoveEndEvent evt:
                  if (startEvt == null) break; // can't move without start event...

                  var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                  var dirX = sfloat.FromRaw(evt.dirX);
                  var dirY = sfloat.FromRaw(evt.dirY);
                  var dir = new float3(dirX, sfloat.Zero, dirY);
                  // var mag =
                  dir = math.normalize(dir);

                  var translation = entityManager.GetComponentData<Translation>(NetworkedPhysics
                     .Entity);
                  var mass = entityManager.GetComponentData<PhysicsMass>(NetworkedPhysics
                     .Entity);

                  var delta = dir;
                  delta.y = sfloat.Zero;

                  var startTime = sfloat.FromRaw(startEvt.startTime);
                  var endTime = sfloat.FromRaw(evt.endTime);

                  var deltaTime = endTime - startTime;

                  var power = GetPowerForDeltaTime(deltaTime);
                  var speed =  power / mass.InverseMass;

                  entityManager.SetComponentData(NetworkedPhysics.Entity, new PhysicsImpulse
                  {
                     Impulse = delta * speed
                  });

                  startEvt = null;
                  break;

               case PlayerMoveStartedEvent moveEvt:
                  Debug.Log("MOVE EVENT RECEIVED: " + moveEvt.dirX);
                  startEvt = moveEvt;
                  // TODO: throw some visualizations here....

                  break;
               default:
                  break;
            }
         }
      }
   }
}