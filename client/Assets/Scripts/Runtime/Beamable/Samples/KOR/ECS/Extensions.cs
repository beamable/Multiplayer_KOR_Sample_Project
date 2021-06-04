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
               position.x.ToSFloat(),
               position.y.ToSFloat(),
               position.z.ToSFloat()
               )
         });
      }

      public static uint ToRawSFloat(this float number)
      {
         return ((sfloat) number).RawValue;
      }

      public static sfloat ToSFloat(this float number, sfloat tolerance=default)
      {
         var lowestTolerance = (sfloat) .01f;
         if (tolerance < lowestTolerance)
         {
            tolerance = lowestTolerance;
         }
         var roughCast = (sfloat) number;
         var cell = math.floor(roughCast / tolerance);
         cell *= tolerance;
         return cell;
      }

      public static sfloat ToSFloat(this uint raw)
      {
         return sfloat.FromRaw(raw);
      }
   }
}