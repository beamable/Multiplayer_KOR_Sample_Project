using System;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Multiplayer;
using Beamable.Samples.KOR.UI;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityS.Transforms;

namespace Beamable.Samples.KOR.Behaviours
{
   public class RaiseEventOnFallBehaviour : MonoBehaviour
   {
      public ConvertToNetworkedPhysics NetworkedPhysics;

      public UnityEvent OnNetworkFall;
      public UnityEvent OnNetworkResurface;

      public UnityEvent OnVisualFall;
      public UnityEvent OnVisualResurface;

      public sfloat TakeDamageAtHeight = -(sfloat).5f;
      private bool _wasNetworkBelow;
      private bool _wasVisualBelow;


      private void Start()
      {
         NetworkController.Instance.Log.CreateNewConsumer(OnNetworkUpdate);
      }

      private void Update()
      {
         var isBelow = NetworkedPhysics.transform.position.y < (float)TakeDamageAtHeight;

         if (isBelow && !_wasVisualBelow)
         {
            OnVisualFall?.Invoke();
         } else if (!isBelow && _wasVisualBelow)
         {
            OnVisualResurface?.Invoke();
         }
         _wasVisualBelow = isBelow;
      }

      private void OnNetworkUpdate(TimeUpdate update)
      {
         if (!NetworkedPhysics.Converted) return;

         // if the networked object is below a certain position, apply damage...
         var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
         var position = entityManager.GetComponentData<Translation>(NetworkedPhysics.Entity);

         var isBelow = (position.Value.y < TakeDamageAtHeight);

         if (isBelow && !_wasNetworkBelow)
         {
            OnNetworkFall?.Invoke();
         } else if (!isBelow && _wasNetworkBelow)
         {
            OnNetworkResurface?.Invoke();
         }
         _wasNetworkBelow = isBelow;

      }

   }
}