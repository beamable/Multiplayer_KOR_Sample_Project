
using System;
using System.Linq;
using Beamable.Samples.Core;
using Unity.Entities;
using UnityEngine;
using UnityS.Mathematics;
using UnityS.Physics;
using BoxCollider = UnityEngine.BoxCollider;
using Collider = UnityS.Physics.Collider;
using SphereCollider = UnityEngine.SphereCollider;

namespace Beamable.Samples.KOR.Multiplayer
{
   [ExecuteAlways]
   public class ConvertToNetworkedPhysics : MonoBehaviour
   {
      public UnityEngine.Collider Collider;

      public bool IsDynamic;

      public sfloat Mass = (sfloat).5f;
      public sfloat Restitution = (sfloat) .5f;

      public bool XRotationLocked;
      public bool YRotationLocked;
      public bool ZRotationLocked;

      public OptionalSfloat ExtraBounce;


      [Header("Internal Baked Soft Float Values")]

      [ReadOnly]
      [SerializeField]
      private uint _x;

      [ReadOnly]
      [SerializeField]
      private uint _y;

      [ReadOnly]
      [SerializeField]
      private uint _z;

      [ReadOnly]
      [SerializeField]
      private uint _rotX;

      [ReadOnly]
      [SerializeField]
      private uint _rotY;

      [ReadOnly]
      [SerializeField]
      private uint _rotZ;

      [ReadOnly]
      [SerializeField]
      private uint _rotW;

      [ReadOnly]
      [SerializeField]
      private uint _colliderCenterX;

      [ReadOnly]
      [SerializeField]
      private uint _colliderCenterY;

      [ReadOnly]
      [SerializeField]
      private uint _colliderCenterZ;

      [ReadOnly]
      [SerializeField]
      private uint _colliderSizeX;

      [ReadOnly]
      [SerializeField]
      private uint _colliderSizeY;

      [ReadOnly]
      [SerializeField]
      private uint _colliderSizeZ;

      [ReadOnly]
      [SerializeField]
      private uint _colliderRadius;

      public bool Converted { get; private set; }

      public Entity Entity { get; private set; }

      private void Start()
      {
         // throw new NotImplementedException();
      }


#if UNITY_EDITOR
      private void Update()
      {
         // only run this at edit time..
         if (Application.IsPlaying(this)) return;

         _x = transform.position.x.ToRawSFloat();
         _y = transform.position.y.ToRawSFloat();
         _z = transform.position.z.ToRawSFloat();
         _rotX = transform.rotation.x.ToRawSFloat();
         _rotY = transform.rotation.y.ToRawSFloat();
         _rotZ = transform.rotation.z.ToRawSFloat();
         _rotW = transform.rotation.w.ToRawSFloat();

         switch (Collider)
         {
            case BoxCollider box:
               _colliderCenterX = box.center.x.ToRawSFloat();
               _colliderCenterY = box.center.y.ToRawSFloat();
               _colliderCenterZ = box.center.z.ToRawSFloat();
               _colliderSizeX = (box.size.x * Collider.transform.lossyScale.x).ToRawSFloat();
               _colliderSizeY = (box.size.y * Collider.transform.lossyScale.y).ToRawSFloat();
               _colliderSizeZ = (box.size.z * Collider.transform.lossyScale.z).ToRawSFloat();
               break;
            case SphereCollider sphere:
               _colliderCenterX = sphere.center.x.ToRawSFloat();
               _colliderCenterY = sphere.center.y.ToRawSFloat();
               _colliderCenterZ = sphere.center.z.ToRawSFloat();
               _colliderRadius = (sphere.radius * sphere.transform.localScale.x).ToRawSFloat();
               break;
         }

      }
      #endif

      // private void OnEnable()
      // {
      //    if (!Application.IsPlaying(this)) return;
      //
      //    // automatically try to convert...
      //    if (GameController.Instance != null && !Converted)
      //    {
      //       Convert(GameController.Instance);
      //    }
      // }

      public static void ConvertAll(GameController controller)
      {

         var unconverted = FindObjectsOfType<ConvertToNetworkedPhysics>().Where(c => !c.Converted).ToList();
         unconverted.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
         foreach (var unconvert in unconverted)
         {
            unconvert.Convert(controller);
         }
      }

      //private async void Awake()
      public void Convert(GameController controller)
      {
         if (!Application.IsPlaying(this)) return;

         if (Converted)
         {
            Debug.LogError("Cannot convert into network physics object because it has already been converted. " + name);
            return;
         }

         if (controller == null)
         {
            Debug.LogError("Cannot convert into network physics object when Game Controller is null. " + name);
            return;
         }

         var allowed = isActiveAndEnabled;
         if (!allowed) return;

         UnityS.Physics.Material material = UnityS.Physics.Material.Default;
         material.Restitution = Restitution;
         material.CollisionResponse = CollisionResponsePolicy.CollideRaiseCollisionEvents;

         var position = new float3(
            _x.ToSFloat(),
            _y.ToSFloat(),
            _z.ToSFloat()
         );

         var rotation = new quaternion(
            _rotX.ToSFloat(),
            _rotY.ToSFloat(),
            _rotZ.ToSFloat(),
            _rotW.ToSFloat()
         );

         // var controller = await GameController.OnInstance;

         var physicsParams = PhysicsParams.Default;
         physicsParams.isDynamic = IsDynamic;
         physicsParams.lockAxis.x = XRotationLocked;
         physicsParams.lockAxis.y = YRotationLocked;
         physicsParams.lockAxis.z = ZRotationLocked;
         physicsParams.mass = Mass;

         BlobAssetReference<Collider> collider;

         switch (Collider)
         {
            case BoxCollider _:
               var size = new float3(
                 _colliderSizeX.ToSFloat(),
                 _colliderSizeY.ToSFloat(),
                 _colliderSizeZ.ToSFloat()
               );
               var center = new float3(
                  _colliderCenterX.ToSFloat(),
                  _colliderCenterY.ToSFloat(),
                  _colliderCenterZ.ToSFloat()
               );
               collider = UnityS.Physics.BoxCollider.Create(
                  new BoxGeometry
                  {
                     Center = center,
                     Size = size,
                     Orientation = quaternion.identity
                  }, CollisionFilter.Default, material
               );
               break;
            case SphereCollider _:
               var sphereCenter = new float3(
                  _colliderCenterX.ToSFloat(),
                  _colliderCenterY.ToSFloat(),
                  _colliderCenterZ.ToSFloat()
               );
               collider = UnityS.Physics.SphereCollider.Create(
                  new SphereGeometry
                  {
                     Center = sphereCenter,
                     Radius = _colliderRadius.ToSFloat()
                  }, CollisionFilter.Default, material);
               break;
            default:
               throw new InvalidOperationException("Collider type isn't supported.");
         }

         Entity = controller.CreatePhysicsBody(position, rotation, collider, physicsParams);

         if (ExtraBounce.HasValue)
         {
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<BouncyTag>(Entity);
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(Entity, new BouncyTag
            {
               Bounce = ExtraBounce.Value
            });
         }

         GameController.Instance.Register(Collider.gameObject, Entity);
         Converted = true;
      }
   }
}