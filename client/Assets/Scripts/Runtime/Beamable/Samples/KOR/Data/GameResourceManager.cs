using Beamable.Samples.Core;
using Beamable.Samples.Core.Components;
using UnityEngine;

namespace Beamable.Samples.KOR.Data
{
   public class GameResourceManager : SingletonMonobehavior<GameResourceManager>
   {
      public MeshRenderer CubePrefab; // TODO: Remove.
      public Material[] colorMaterials; // TODO: Remove.
   }
}