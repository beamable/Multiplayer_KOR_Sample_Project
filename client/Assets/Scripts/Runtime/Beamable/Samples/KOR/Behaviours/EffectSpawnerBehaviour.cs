using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
   public class EffectSpawnerBehaviour : MonoBehaviour
   {
      public GameObject Prefab;
      public Transform SpawnLocation;
      public float Time = 2;

      public void SpawnEffect()
      {
         var effect = Instantiate(Prefab);
         effect.transform.position = SpawnLocation.position;
         Destroy(effect, Time);
      }

   }
}