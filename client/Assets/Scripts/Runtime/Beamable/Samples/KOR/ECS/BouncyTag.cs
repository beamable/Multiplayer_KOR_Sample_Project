using Unity.Entities;

namespace Beamable.Samples.KOR.Multiplayer
{
   public struct BouncyTag : IComponentData
   {
      public sfloat Bounce;
      public sfloat Shield;
   }
}