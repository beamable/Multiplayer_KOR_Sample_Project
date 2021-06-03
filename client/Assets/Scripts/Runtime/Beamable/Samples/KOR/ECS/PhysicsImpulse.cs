using Unity.Entities;
using UnityS.Mathematics;

namespace Beamable.Samples.KOR.Multiplayer
{
   public struct PhysicsImpulse : IComponentData
   {
      public float3 Impulse;
   }
}