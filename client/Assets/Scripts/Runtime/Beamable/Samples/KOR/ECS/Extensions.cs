using Unity.Entities;
using UnityEngine;
using UnityS.Mathematics;
using UnityS.Transforms;

namespace Beamable.Samples.KOR.Multiplayer
{
   public static class Extensions
   {
      public static void SetPhysicsPosition(this Transform transform, Vector3 position)
      {
         var entity = GameController.Instance.GetEntity(transform.gameObject);
         World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, new Translation()
         {
            Value = new float3(
               (sfloat)position.x,
               (sfloat)position.y,
               (sfloat)position.z
               )
         });
      }
   }
}