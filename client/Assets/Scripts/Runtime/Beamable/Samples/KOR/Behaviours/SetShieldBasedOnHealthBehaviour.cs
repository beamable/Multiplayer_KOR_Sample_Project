using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Multiplayer;
using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
   public class SetShieldBasedOnHealthBehaviour : MonoBehaviour
   {
      public ConvertToNetworkedPhysics NetworkedPhysics;

      public HealthBehaviour Health;

      void Start()
      {
         NetworkController.Instance.Log.CreateNewConsumer(NetworkUpdate);
      }

      void NetworkUpdate(TimeUpdate update)
      {
         if (!NetworkedPhysics.Converted) return;

         var entityManager = GameController.Instance.EntityManager;
         var data = entityManager.GetComponentData<BouncyTag>(NetworkedPhysics.Entity);
         data.Shield = Health.HealthRatio;
         entityManager.SetComponentData(NetworkedPhysics.Entity, data);
      }
   }
}