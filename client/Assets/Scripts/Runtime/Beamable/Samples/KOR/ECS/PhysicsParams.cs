using UnityS.Mathematics;

namespace Beamable.Samples.KOR.Multiplayer
{
   public struct PhysicsParams
   {
      public float3 startingLinearVelocity;
      public float3 startingAngularVelocity;
      public sfloat mass;
      public bool isDynamic;
      public sfloat linearDamping;
      public sfloat angularDamping;

      public static PhysicsParams Default => new PhysicsParams
      {
         startingLinearVelocity = float3.zero,
         startingAngularVelocity = float3.zero,
         mass = sfloat.One,
         isDynamic = true,
         linearDamping = sfloat.FromRaw(0x3c23d70a),
         angularDamping = sfloat.FromRaw(0x3d4ccccd)
      };
   }
}