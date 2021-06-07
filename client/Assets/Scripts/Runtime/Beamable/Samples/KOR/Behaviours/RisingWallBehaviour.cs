using System;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Multiplayer;
using Unity.Entities;
using UnityEngine;
using UnityS.Mathematics;
using UnityS.Transforms;

namespace Beamable.Samples.KOR.Behaviours
{
   /// <summary>
   /// This could also be implemented directly with ECS, by adding a new component to the entity.
   /// The component would contain the data for the motion.
   /// A new system would be created that operates on the entity, the new component, and the Translation
   ///
   /// This is left as a MonoBehaviour to demonstrate how Unity ECS can integrate with traditional Unity
   /// </summary>
   public class RisingWallBehaviour : MonoBehaviour
   {
      public ConvertToNetworkedPhysics NetworkedPhysics;

      public float Frequency = 1;
      public float Amplitude = 1;
      public float Offset = 0;
      public bool UseAbs = false;
      public bool FlipY = false;

      private float3 _startingPosition;
      private bool _first = true;

      void Start()
      {
         NetworkController.Instance.Log.CreateNewConsumer(OnNetworkUpdate);
      }

      private void OnNetworkUpdate(TimeUpdate update)
      {
         if (!NetworkedPhysics.Converted) return;


         var translation = GameController.Instance.EntityManager.GetComponentData<Translation>(NetworkedPhysics.Entity);
         if (_first)
         {
            _first = false;
            _startingPosition = translation.Value;
         }

         var freq = (sfloat) Frequency;
         var amp = (sfloat) Amplitude;
         var t = (sfloat) (update.ElapsedTime * 6.28f);
         var offset = (sfloat) Offset;

         var rise = amp * math.sin(t * freq + offset);
         if (UseAbs)
         {
            rise = math.abs(rise);
         }

         if (FlipY)
         {
            rise = -rise;
         }
         translation.Value.y = _startingPosition.y + rise;

         GameController.Instance.EntityManager.SetComponentData(NetworkedPhysics.Entity, translation);
      }
   }
}