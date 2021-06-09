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

      public MovePreviewBehaviour PreviewBehaviour;

      public ConvertToNetworkedPhysics NetworkedPhysics;

      public int MaxPower = 22;
      public int MinPower = 8;

      public float PowerGrowthSlope = 10;
      public float PowerGrowthExp = .9f;

      [ReadOnly] public int consumerId;

      [ReadOnly] public PlayerMoveStartedEvent startEvt;

      public float3 direction;
      public sfloat deltaTime;
      public sfloat magnitude;

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

         power += minPower;
         if (power > maxPower)
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

      private void SetDirection(uint x, uint y)
      {
         var dirX = sfloat.FromRaw(x);
         var dirY = sfloat.FromRaw(y);
         var dir = new float3(dirX, sfloat.Zero, dirY);
         // var mag =
         magnitude = math.length(dir);
         direction = math.normalize(dir);
      }

      private void SetDeltaTime(uint time)
      {
         var startTime = sfloat.FromRaw(startEvt.startTime);
         var endTime = sfloat.FromRaw(time);

         deltaTime = endTime - startTime;
      }

      private void OnNetworkUpdate(TimeUpdate timeUpdate)
      {

         var tick = (long) (timeUpdate.ElapsedTime * NetworkController.NetworkFramesPerSecond);
         foreach (var message in timeUpdate.Events)
         {
            if (message.PlayerDbid != AvatarView.playerDbid)
               continue; // this message doesn't belong to the player we care about
            switch (message)
            {
               case PlayerMoveProgressEvent progressEvt:
                  if (startEvt == null) break; // can't move without start event...

                  SetDirection(progressEvt.dirX, progressEvt.dirY);
                  SetDeltaTime(progressEvt.endTime);

                  PreviewBehaviour?.Set(true, new Vector3((float) direction.x, 0, (float) direction.z),
                     (float) GetPowerRatioForDeltaTime(deltaTime));
                  break;

               case PlayerMoveEndEvent evt:
                  // start playing animation right away...
                  var isLocal = AvatarView.playerDbid == NetworkController.Instance.LocalDbid;
                  if (!isLocal)
                  {
                     AvatarView.PlayAnimationAttack01();
                  }

                  var sentAt = evt.endTime.ToSFloat();
                  var currTime = (sfloat)timeUpdate.ElapsedTime;

                  var latency = (currTime - sentAt);
                  var forcedDelay = (sfloat) 2f; // half a second of forced delay...
                  var paddedDelay = math.max(sfloat.Zero, forcedDelay - latency);

                  var paddedDelayInTicks = (long) (paddedDelay * (sfloat) NetworkController.NetworkFramesPerSecond);

                  Debug.Log("Pad. " + latency + " / " + paddedDelay + " = " + paddedDelayInTicks);
                  timeUpdate.ScheduleAction(paddedDelayInTicks, () =>
                  {
                     HandleMotionEndEvent(evt);
                  });
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

      private void HandleMotionEndEvent(PlayerMoveEndEvent evt)
      {
         if (startEvt == null) return; // can't move without start event...

         var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

         SetDirection(evt.dirX, evt.dirY);
         SetDeltaTime(evt.endTime);

         var mass = entityManager.GetComponentData<PhysicsMass>(NetworkedPhysics
            .Entity);

         var power = GetPowerForDeltaTime(deltaTime);
         var speed = power / mass.InverseMass;

         entityManager.SetComponentData(NetworkedPhysics.Entity, new PhysicsImpulse
         {
            Impulse = direction * magnitude * speed
         });


         if (direction.x.IsNaN() || magnitude.IsNaN() || deltaTime.IsNaN() || speed.IsNaN())
         {
            Debug.Log("There was a NaN movement tick. Reseting to zero");
            magnitude = sfloat.Zero;
            direction = new float3(sfloat.One, sfloat.Zero, sfloat.Zero);
            deltaTime = sfloat.Zero;
            startEvt = null;
            return;
         }

         entityManager.SetComponentData(NetworkedPhysics.Entity, new PhysicsImpulse
         {
            Impulse = direction * magnitude * speed
         });
         PreviewBehaviour?.Set(false, Vector3.right, 0);
         startEvt = null;
      }
   }
}