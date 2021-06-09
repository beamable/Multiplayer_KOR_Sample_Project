using System;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.Core;
using Beamable.Samples.KOR.Multiplayer.Events;
using Beamable.Samples.KOR.Views;
using Unity.Entities;
using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
   public class PlayerInputBehaviour : MonoBehaviour
   {
      public AvatarMotionBehaviour MotionBehaviour;

      private Vector3 _lastHit;

      public float VisualArrowLength;

      public MovePreviewBehaviour PreviewBehaviour;

      public AvatarView AvatarView;

      public LayerMask CollisionMask;

      [ReadOnly]
      public Vector3 direction;

      [ReadOnly]
      public bool isPowering;

      [ReadOnly]
      public double startedPoweringAt;

      [ReadOnly]
      public float powerRatio;

      [ReadOnly]
      public long sentOnTick;

      private void OnDrawGizmos()
      {
         if (!isPowering) return;
         var start = transform.position + Vector3.up * 1;
         var end = start + direction * VisualArrowLength + Vector3.up * 1;

         Gizmos.color = Color.gray;
         Gizmos.DrawLine(start, end);

         Gizmos.color = Color.red;
         Gizmos.DrawLine(start, start + ((end - start) * powerRatio));
      }

      private void Update()
      {
         TrackPower();
         CheckForInputs();

         PreviewBehaviour.Set(isPowering, direction, powerRatio);
      }

      void TrackPower()
      {
         if (!isPowering)
         {
            powerRatio = 0;
            return;
         }

         var now = (sfloat)World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;
         var started = (sfloat) startedPoweringAt;

         var dt = now - started;
         powerRatio = (float)MotionBehaviour.GetPowerRatioForDeltaTime(dt);

      }

      void CheckForInputs()
      {
         // where on the floor is the player clicking?
         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if (!Physics.Raycast(ray, out var hit, CollisionMask.value)) return;



         direction = (hit.point - transform.position).normalized;
         _lastHit = hit.point;


         if (Input.GetMouseButtonDown(0))
         {
            // AvatarView.PlayAnimationAttack01();
            startedPoweringAt = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;
            var time = (sfloat)World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;
            isPowering = true;
            NetworkController.Instance.SendNetworkMessage(new PlayerMoveStartedEvent(time, direction));
            sentOnTick = NetworkController.HighestSeenNetworkFrame;
         } else if (Input.GetMouseButtonUp(0))
         {

            AvatarView.PlayAnimationAttack01(); // TODO: How to get other players to play the animation before they move?

            var time = (sfloat)World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;
            isPowering = false;
            NetworkController.Instance.SendNetworkMessage(new PlayerMoveEndEvent(time, direction));
            sentOnTick = NetworkController.HighestSeenNetworkFrame;
         } else if (isPowering && sentOnTick != NetworkController.HighestSeenNetworkFrame)
         {
            // we only need to send one message per tick...
            var time = (sfloat)World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;
            NetworkController.Instance.SendNetworkMessage(new PlayerMoveProgressEvent(time, direction));
            sentOnTick = NetworkController.HighestSeenNetworkFrame;
         }

      }
   }
}