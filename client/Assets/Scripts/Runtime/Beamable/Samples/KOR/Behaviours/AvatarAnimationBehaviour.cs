using System;
using Beamable.Samples.KOR.Views;
using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
   public class AvatarAnimationBehaviour : MonoBehaviour
   {

      public AvatarView Avatar;
      public AvatarMotionBehaviour Motion;
      public PlayerInputBehaviour Input;


      private Vector3 _lastPosition;

      private Vector3 actualDirection;
      private Vector3 directionVelocity;


      private void Update()
      {

         var position = Avatar.Model.position;
         var diff = (_lastPosition - position);
         _lastPosition = position;

         var mag = diff.magnitude;
         var dir = diff.normalized;

         var isStill = mag < .05f;
         // var isWalking = mag >= 1 && mag < 2;
         var isRunning = mag >= .05f;
         if (isStill)
         {
            Avatar.PlayAnimationIdle();
         } else if (false)
         {
            Avatar.PlayAnimationWalkForward();
         } else if (isRunning)
         {
            Avatar.PlayAnimationRunForward();
         }


         var dirZ = (float) Motion.direction.z;
         var dirX = (float) Motion.direction.x;
         var targetDirection = new Vector3(dirZ, 0, dirX);
         if (Input)
         {
            targetDirection = Input.direction;
         }

         targetDirection = targetDirection.normalized;
         actualDirection = Vector3.SmoothDamp(actualDirection, targetDirection, ref directionVelocity, .1f, 5);
         Avatar.Model.transform.rotation = Quaternion.AngleAxis(  Mathf.Rad2Deg * Mathf.Atan2(actualDirection.z, actualDirection.x), Vector3.up);



      }


   }
}